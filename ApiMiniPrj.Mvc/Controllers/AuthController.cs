using ApiMiniPrj.Application.DTOs.Auth;
using ApiMiniPrj.Mvc.Common;
using ApiMiniPrj.Mvc.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace ApiMiniPrj.Mvc.Controllers
{
    public class AuthController(IHttpClientFactory httpClientFactory) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View(new AuthVM());
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthVM vm)
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                [nameof(LoginDto.EmailOrUsername)] = vm.Login.EmailOrUsername ?? string.Empty,
                [nameof(LoginDto.Password)] = vm.Login.Password ?? string.Empty,
                [nameof(LoginDto.RememberMe)] = vm.Login.RememberMe.ToString()
            });

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync("api/auth/login", content);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "API service is not reachable. Please make sure the API project is running.");
                return View(vm);
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                return View(vm);
            }

            var loginResult = await response.Content.ReadFromJsonAsync<ResponseDto>();
            if (loginResult is null || string.IsNullOrWhiteSpace(loginResult.Token))
            {
                ModelState.AddModelError(string.Empty, "Login succeeded, but no token was received from the API.");
                return View(vm);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = vm.Login.RememberMe ? DateTimeOffset.UtcNow.AddDays(15) : null
            };

            Response.Cookies.Append("AccessToken", loginResult.Token, cookieOptions);
            Response.Cookies.Append("RefreshToken", loginResult.RefreshToken, cookieOptions);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var vm = new AuthVM();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(AuthVM vm)
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            HttpResponseMessage response;

            try
            {
                response = await client.PostAsJsonAsync("api/auth/register", new RegisterDto
                {
                    Email = vm.Register.Email,
                    Password = vm.Register.Password,
                    RePassword = vm.Register.RePassword,
                    UserName = vm.Register.UserName,
                    FullName = vm.Register.FullName,
                    AcceptTerms = vm.Register.AcceptTerms
                });
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "API service is not reachable. Please make sure the API project is running.");
                return View(vm);
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                return View(vm);
            }

            var registerResult = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
            if (registerResult is null || string.IsNullOrEmpty(registerResult.Token))
            {
                ModelState.AddModelError(string.Empty, "Registration succeeded, but no confirmation token was received from the API.");
                return View(vm);
            }

            var confirmationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(registerResult.Token));
            return RedirectToAction(nameof(ConfirmEmail), new
            {
                email = vm.Register.Email,
                token = confirmationToken
            });
        }

        [HttpGet]
        public IActionResult CheckEmail()
        {
            return View(new AuthVM());
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string? email, string? token)
        {
            var vm = new AuthVM
            {
                ConfirmEmail = new ConfirmEmailDto
                {
                    Email = email ?? string.Empty,
                    Token = token ?? string.Empty
                }
            };

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError(string.Empty, "Email confirmation link is missing email or token.");
                return View("CheckEmail", vm);
            }

            var confirmResult = await ConfirmEmailWithApiAsync(email, token);
            if (!confirmResult.IsSuccess)
            {
                AddApiErrors(confirmResult.ErrorMessage);
                return View("CheckEmail", vm);
            }

            TempData["SuccessMessage"] = "Email confirmed successfully! You can now log in.";
            return View("CheckEmail", vm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(AuthVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ConfirmEmail.Email) || string.IsNullOrWhiteSpace(vm.ConfirmEmail.Token))
            {
                ModelState.AddModelError(string.Empty, "Email and token are required.");
                return View("CheckEmail", vm);
            }

            var confirmResult = await ConfirmEmailWithApiAsync(vm.ConfirmEmail.Email, vm.ConfirmEmail.Token);
            if (!confirmResult.IsSuccess)
            {
                AddApiErrors(confirmResult.ErrorMessage);
                return View("CheckEmail", vm);
            }

            TempData["SuccessMessage"] = "Email confirmed successfully! You can now log in.";
            return View("CheckEmail", vm);
        }

        private async Task<(bool IsSuccess, string ErrorMessage)> ConfirmEmailWithApiAsync(string email, string encodedToken)
        {
            string token;
            try
            {
                token = Encoding.UTF8.GetString(Convert.FromBase64String(encodedToken));
            }
            catch (FormatException)
            {
                return (false, "Invalid email confirmation token format.");
            }

            var client = httpClientFactory.CreateClient("ApiClient");
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                [nameof(ConfirmEmailDto.Email)] = email,
                [nameof(ConfirmEmailDto.Token)] = token
            });

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync("api/auth/confirm-email", content);

            if (!response.IsSuccessStatusCode)
            {
                return (false, await ReadApiErrorAsync(response));
            }

            return (true, string.Empty);
        }

        private void AddApiErrors(string errorMessage)
        {
            foreach (var error in errorMessage.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private static async Task<string> ReadApiErrorAsync(HttpResponseMessage response)
        {
            return await ApiErrorReader.ReadAsync(response);
        }
    }
}

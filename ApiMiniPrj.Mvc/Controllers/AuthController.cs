using ApiMiniPrj.Application.DTOs.Auth;
using ApiMiniPrj.Mvc.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ApiMiniPrj.Mvc.Controllers
{
    public class AuthController(IHttpClientFactory httpClientFactory) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            AuthVM vm = new AuthVM();
            var response = await client.GetAsync("api/auth/register");

            return View(vm.Register);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/auth/register", registerDto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                return View(registerDto);
            }
            var token = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
            if (token is null)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                return View(registerDto);
            }


            var result = await client.GetAsync($"api/auth/confirm-email?token={token.Token}");

            return RedirectToAction("CheckEmail");
        }
    }
}

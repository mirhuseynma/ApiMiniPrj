using ApiMiniPrj.Application.DTOs.Organizers;
using ApiMiniPrj.Mvc.Common;
using ApiMiniPrj.Mvc.Models.Organizers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace ApiMiniPrj.Mvc.Controllers
{
    public class OrganizerController(IHttpClientFactory httpClientFactory) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new OrganizerIndexVM();
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync("api/oraganizer");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "API service is not reachable. Please make sure the API project is running.");
                return View(vm);
            }

            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                return View(vm);
            }

            vm.Organizers = await response.Content.ReadFromJsonAsync<List<GetOrganizerDto>>() ?? [];
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"api/oraganizer/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            var organizer = await response.Content.ReadFromJsonAsync<GetOrganizerDto>();
            if (organizer is null)
            {
                return NotFound();
            }

            return View(organizer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new OrganizerCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrganizerCreateVM vm)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            using var content = BuildOrganizerContent(vm.FullName, vm.Email, vm.PhoneNumber, vm.Logo);
            var response = await client.PostAsync("api/oraganizer", content);
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                return View(vm);
            }

            TempData["SuccessMessage"] = "Organizer created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"api/oraganizer/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            var organizer = await response.Content.ReadFromJsonAsync<GetOrganizerDto>();
            if (organizer is null)
            {
                return NotFound();
            }

            return View(new OrganizerUpdateVM
            {
                Id = organizer.Id,
                FullName = organizer.FullName,
                Email = organizer.Email,
                PhoneNumber = organizer.PhoneNumber,
                LogoUrl = organizer.LogoUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrganizerUpdateVM vm)
        {
            if (id != vm.Id)
            {
                return BadRequest();
            }

            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            using var content = BuildOrganizerContent(vm.FullName, vm.Email, vm.PhoneNumber, vm.Logo);
            var response = await client.PutAsync($"api/oraganizer/{id}", content);
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                return View(vm);
            }

            TempData["SuccessMessage"] = "Organizer updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.DeleteAsync($"api/oraganizer/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Organizer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private HttpClient? CreateAuthorizedClient()
        {
            var token = Request.Cookies["AccessToken"];
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var client = httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private IActionResult? HandleAuthResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return null;
        }

        private static MultipartFormDataContent BuildOrganizerContent(string? fullName, string? email, string? phoneNumber, IFormFile? logo)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(fullName ?? string.Empty), nameof(OrganizerCreateDto.FullName) },
                { new StringContent(email ?? string.Empty), nameof(OrganizerCreateDto.Email) },
                { new StringContent(phoneNumber ?? string.Empty), nameof(OrganizerCreateDto.PhoneNumber) }
            };

            if (logo is not null && logo.Length > 0)
            {
                var fileContent = new StreamContent(logo.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(logo.ContentType);
                content.Add(fileContent, nameof(OrganizerCreateDto.Logo), logo.FileName);
            }

            return content;
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

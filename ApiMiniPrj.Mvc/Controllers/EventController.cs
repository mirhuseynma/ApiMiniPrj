using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Application.DTOs.Organizers;
using ApiMiniPrj.Mvc.Common;
using ApiMiniPrj.Mvc.Models.Events;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace ApiMiniPrj.Mvc.Controllers
{
    public class EventController(IHttpClientFactory httpClientFactory) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new GetEventsVM();
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync("api/event");
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

            vm.Events = await response.Content.ReadFromJsonAsync<List<GetEventDto>>() ?? [];
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"api/event/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            var eventDto = await response.Content.ReadFromJsonAsync<GetEventDto>();
            if (eventDto is null)
            {
                return NotFound();
            }

            return View(eventDto);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new CreateEventVM();
            vm.Organizers = await GetOrganizerOptionsAsync(client);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventVM vm)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            using var content = BuildEventContent(vm.Title, vm.Description, vm.Date, vm.Location, vm.OrganizerId, vm.BannerImage);
            var response = await client.PostAsync("api/event", content);
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                vm.Organizers = await GetOrganizerOptionsAsync(client);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Event created successfully.";
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

            var response = await client.GetAsync($"api/event/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            var eventDto = await response.Content.ReadFromJsonAsync<GetEventDto>();
            if (eventDto is null)
            {
                return NotFound();
            }

            var vm = new UpdateEventVM
            {
                Id = eventDto.Id,
                Title = eventDto.Title,
                Description = eventDto.Description,
                Date = eventDto.Date,
                Location = eventDto.Location,
                OrganizerId = eventDto.Organizer?.Id,
                BannerImageUrl = eventDto.BannerImageUrl
            };
            vm.Organizers = await GetOrganizerOptionsAsync(client);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEventVM vm)
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

            using var content = BuildEventContent(vm.Title, vm.Description, vm.Date, vm.Location, vm.OrganizerId, vm.BannerImage);
            var response = await client.PutAsync($"api/event/{id}", content);
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                vm.Organizers = await GetOrganizerOptionsAsync(client);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Event updated successfully.";
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

            var response = await client.DeleteAsync($"api/event/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Event deleted successfully.";
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

        private static MultipartFormDataContent BuildEventContent(
            string? title,
            string? description,
            DateTime? date,
            string? location,
            int? organizerId,
            IFormFile? bannerImage)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(title ?? string.Empty), nameof(EventCreateDto.Title) },
                { new StringContent(description ?? string.Empty), nameof(EventCreateDto.Description) },
                { new StringContent(date?.ToString("O") ?? string.Empty), nameof(EventCreateDto.Date) },
                { new StringContent(location ?? string.Empty), nameof(EventCreateDto.Location) },
                { new StringContent(organizerId?.ToString() ?? string.Empty), nameof(EventCreateDto.OrganizerId) }
            };

            if (bannerImage is not null && bannerImage.Length > 0)
            {
                var fileContent = new StreamContent(bannerImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(bannerImage.ContentType);
                content.Add(fileContent, nameof(EventCreateDto.BannerImage), bannerImage.FileName);
            }

            return content;
        }

        private static async Task<List<EventOrganizerOptionVM>> GetOrganizerOptionsAsync(HttpClient client)
        {
            var response = await client.GetAsync("api/oraganizer");
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            var organizers = await response.Content.ReadFromJsonAsync<List<GetOrganizerDto>>() ?? [];
            return [.. organizers
                .Select(organizer => new EventOrganizerOptionVM
                {
                    Id = organizer.Id,
                    FullName = organizer.FullName
                })];
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

using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Application.DTOs.Tickets;
using ApiMiniPrj.Mvc.Common;
using ApiMiniPrj.Mvc.Models.Tickets;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiMiniPrj.Mvc.Controllers
{
    public class TicketController(IHttpClientFactory httpClientFactory) : Controller
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public async Task<IActionResult> Index()
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new TicketIndexVM();
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync("api/ticket");
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

            vm.Tickets = await response.Content.ReadFromJsonAsync<List<GetTicketDto>>(JsonOptions) ?? [];
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"api/ticket/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            var ticket = await response.Content.ReadFromJsonAsync<GetTicketDto>(JsonOptions);
            if (ticket is null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var vm = new TicketCreateVM
            {
                Events = await GetEventOptionsAsync(client)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateVM vm)
        {
            var client = CreateAuthorizedClient();
            if (client is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            using var content = BuildTicketContent(vm.Type, vm.Quantity, vm.Price, vm.IsAvaiable, vm.EventId);
            var response = await client.PostAsync("api/ticket", content);
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                vm.Events = await GetEventOptionsAsync(client);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Ticket created successfully.";
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

            var response = await client.GetAsync($"api/ticket/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            var ticket = await response.Content.ReadFromJsonAsync<GetTicketDto>(JsonOptions);
            if (ticket is null)
            {
                return NotFound();
            }

            var vm = new TicketUpdateVM
            {
                Id = ticket.Id,
                Type = ticket.Type,
                Quantity = ticket.Quantity,
                Price = ticket.Price,
                IsAvaiable = ticket.IsAvaiable,
                EventId = ticket.Event?.Id,
                Events = await GetEventOptionsAsync(client)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketUpdateVM vm)
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

            using var content = BuildTicketContent(vm.Type, vm.Quantity, vm.Price, vm.IsAvaiable, vm.EventId);
            var response = await client.PutAsync($"api/ticket/{id}", content);
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                AddApiErrors(await ReadApiErrorAsync(response));
                vm.Events = await GetEventOptionsAsync(client);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Ticket updated successfully.";
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

            var response = await client.DeleteAsync($"api/ticket/{id}");
            if (HandleAuthResponse(response) is IActionResult authResult)
            {
                return authResult;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Ticket deleted successfully.";
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

        private static MultipartFormDataContent BuildTicketContent(
            object? type,
            object? quantity,
            object? price,
            object? isAvaiable,
            object? eventId)
        {
            return new MultipartFormDataContent
            {
                { new StringContent(type?.ToString() ?? string.Empty), nameof(TicketCreateDto.Type) },
                { new StringContent(quantity?.ToString() ?? string.Empty), nameof(TicketCreateDto.Quantity) },
                { new StringContent(Convert.ToString(price, CultureInfo.InvariantCulture) ?? string.Empty), nameof(TicketCreateDto.Price) },
                { new StringContent(isAvaiable?.ToString() ?? string.Empty), nameof(TicketCreateDto.IsAvaiable) },
                { new StringContent(eventId?.ToString() ?? string.Empty), nameof(TicketCreateDto.EventId) }
            };
        }

        private static async Task<List<TicketEventOptionVM>> GetEventOptionsAsync(HttpClient client)
        {
            var response = await client.GetAsync("api/event");
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            var events = await response.Content.ReadFromJsonAsync<List<GetEventDto>>() ?? [];
            return events
                .Select(eventItem => new TicketEventOptionVM
                {
                    Id = eventItem.Id,
                    Title = eventItem.Title
                })
                .ToList();
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

using ApiMiniPrj.Application.DTOs.Events;
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
            var token = Request.Cookies["AccessToken"];
            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            GetEventsVM vm = new();
            var response = await client.GetAsync("api/event");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return Forbid();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Events could not be loaded.");
                return View(vm);
            }

            vm.Events = await response.Content.ReadFromJsonAsync<List<GetEventDto>>() ?? [];
            return View(vm);
        }
    }
}

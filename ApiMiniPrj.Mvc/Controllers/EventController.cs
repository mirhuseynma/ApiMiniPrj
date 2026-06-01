using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Mvc.Models.Events;
using Microsoft.AspNetCore.Mvc;

namespace ApiMiniPrj.Mvc.Controllers
{
    public class EventController(IHttpClientFactory httpClientFactory) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            GetEventsVM vm = new();
            var events = await client.GetFromJsonAsync<List<GetEventDto>>("api/event") ?? [];
            vm.Events = events;
            return View(vm);
        }
    }
}

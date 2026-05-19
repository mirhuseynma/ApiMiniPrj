using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Application.Interfaces.Common;
using ApiMiniPrj.Application.Interfaces.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMiniPrj.Api.Controllers.Events
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IAppDbContext _context;
        private readonly IFileStorageService _filestorageService;

        public EventController(IEventService eventService, IAppDbContext appDbContext, IFileStorageService filestorageService)
        {
            _eventService = eventService;
            _context = appDbContext;
            _filestorageService = filestorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] EventCreateDto eventCreateDto)
        {
            await _eventService.CreateEventAsync(eventCreateDto);
            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var eventDto = await _eventService.GetEventById(id);
            return Ok(eventDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] EventUpdateDto eventUpdateDto)
        {
            await _eventService.UpdateEventAsync(id, eventUpdateDto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok();
        }

        [HttpPost("{Id:int}/bannerImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddBannerImage(int Id, [FromForm] EventBannerImageUploadDto request)
        {
            if (request.BannerImage is null || request.BannerImage.Length == 0)
            {
                return BadRequest("Banner image is required.");
            }

            await _eventService.AddBannerImageAsync(Id, request.BannerImage);
            return Ok();
        }
    }
}

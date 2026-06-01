
namespace ApiMiniPrj.Api.Controllers.Events
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IValidator<EventCreateDto> _createValidator;
        private readonly IValidator<EventUpdateDto> _updateValidator;
        private readonly IValidator<EventBannerImageUploadDto> _bannerImageUploadValidator;

        public EventController(IEventService eventService, IValidator<EventCreateDto> createValidator, IValidator<EventUpdateDto> updateValidator, IValidator<EventBannerImageUploadDto> bannerImageUploadValidator)
        {
            _eventService = eventService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _bannerImageUploadValidator = bannerImageUploadValidator;
        }

        //[Authorize(Policy = "Permissions.Events.View")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpPost]
        [Authorize(Policy = "Permissions.Events.Create")]
        public async Task<IActionResult> Post([FromForm] EventCreateDto eventCreateDto)
        {
            var validationResult = await _createValidator.ValidateAsync(eventCreateDto);
            if (!validationResult.IsValid) return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            
            await _eventService.CreateEventAsync(eventCreateDto);
            return Ok();
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Permissions.Events.View")]
        public async Task<IActionResult> Get(int id)
        {
            var eventDto = await _eventService.GetEventById(id);
            return Ok(eventDto);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Permissions.Events.Edit")]
        public async Task<IActionResult> Put(int id, [FromForm] EventUpdateDto eventUpdateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(eventUpdateDto);
            if (!validationResult.IsValid) return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            await _eventService.UpdateEventAsync(id, eventUpdateDto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Permissions.Events.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok();
        }

        [HttpPost("{Id:int}/bannerImage")]
        [Authorize(Policy = "Permissions.Events.AddBanner")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddBannerImage(int Id, [FromForm] EventBannerImageUploadDto request)
        {
            var validationResult = await _bannerImageUploadValidator.ValidateAsync(request);
            if (!validationResult.IsValid) return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            await _eventService.AddBannerImageAsync(Id, request.BannerImage!);
            return Ok();
        }
    }
}


namespace ApiMiniPrj.Api.Controllers.Organizers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OraganizerController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;
        private readonly IValidator<OrganizerCreateDto> _createValidator;
        private readonly IValidator<OrganizerUpdateDto> _updateValidator;
        private readonly IValidator<OrganizerUploadLogo> _uploadLogoValidator;

        public OraganizerController(IOrganizerService organizerService, IValidator<OrganizerCreateDto> createValidator, IValidator<OrganizerUpdateDto> updateValidator, IValidator<OrganizerUploadLogo> uploadLogoValidator)
        {
            _organizerService = organizerService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _uploadLogoValidator = uploadLogoValidator;
        }

        [HttpGet]
        [Authorize(Policy = "Permissions.Organizers.View")]
        public async Task<IActionResult> Get()
        {
            var organizers = await _organizerService.GetAllOrganizersAsync();
            return Ok(organizers);
            
        }

        [HttpPost]
        [Authorize(Policy = "Permissions.Organizers.Create")]
        public async Task<IActionResult> Post([FromForm] OrganizerCreateDto organizerCreateDto)
        {
            var validationResult = await _createValidator.ValidateAsync(organizerCreateDto);
            if (!validationResult.IsValid) return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            
            await _organizerService.CreateOrganizerAsync(organizerCreateDto);
            return Ok();
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Permissions.Organizers.View")]
        public async Task<IActionResult> Get(int id)
        {
            var organizer = await _organizerService.GetOrganizerByIdAsync(id);
            return Ok(organizer);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Permissions.Organizers.Edit")]
        public async Task<IActionResult> Put(int id, [FromForm] OrganizerUpdateDto organizerUpdateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(organizerUpdateDto);
            if (!validationResult.IsValid) return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            await _organizerService.UpdateOrganizerAsync(id, organizerUpdateDto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Permissions.Organizers.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _organizerService.DeleteOrganizerAsync(id);
            return Ok();
        }

        [HttpPost("{id:int}/logo")]
        [Authorize(Policy = "Permissions.Organizers.AddLogo")]
        public async Task<IActionResult> UploadLogo(int id, [FromForm] OrganizerUploadLogo logo)
        {
            var validationResult = await _uploadLogoValidator.ValidateAsync(logo);
            if (!validationResult.IsValid) return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            if (logo.Logo is null || logo.Logo.Length == 0)
            {
                return BadRequest("Logo file is required.");
            }
            await _organizerService.OrganizerUploadLogo(id, logo.Logo);
            return Ok();
        }
    }
}

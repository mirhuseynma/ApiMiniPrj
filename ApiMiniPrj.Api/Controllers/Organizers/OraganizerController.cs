
namespace ApiMiniPrj.Api.Controllers.Organizers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OraganizerController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;
        private readonly IAppDbContext _context;
        private readonly IFileStorageService _filestorageService;

        public OraganizerController(IOrganizerService organizerService, IAppDbContext appDbContext, IFileStorageService filestorageService)
        {
            _organizerService = organizerService;
            _context = appDbContext;
            _filestorageService = filestorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var organizers = await _organizerService.GetAllOrganizersAsync();
            return Ok(organizers);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] OrganizerCreateDto organizerCreateDto)
        {
            await _organizerService.CreateOrganizerAsync(organizerCreateDto);
            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var organizer = await _organizerService.GetOrganizerByIdAsync(id);
            return Ok(organizer);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] OrganizerUpdateDto organizerUpdateDto)
        {
            await _organizerService.UpdateOrganizerAsync(id, organizerUpdateDto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _organizerService.DeleteOrganizerAsync(id);
            return Ok();
        }

        [HttpPost("{id:int}/logo")]
        public async Task<IActionResult> UploadLogo(int id, [FromForm] OrganizerUploadLogo logo)
        {
            if (logo.Logo is null || logo.Logo.Length == 0)
            {
                return BadRequest("Logo file is required.");
            }
            await _organizerService.OrganizerUploadLogo(id, logo.Logo);
            return Ok();
        }
    }
}

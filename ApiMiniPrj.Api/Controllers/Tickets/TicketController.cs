
namespace ApiMiniPrj.Api.Controllers.Tickets
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IAppDbContext _context;
        private readonly IValidator<TicketCreateDto> _createValidator;
        private readonly IValidator<TicketUpdateDto> _updateValidator;

        public TicketController(ITicketService ticketService, IAppDbContext appDbContext, IValidator<TicketCreateDto> createValidator, IValidator<TicketUpdateDto> updateValidator)
        {
            _ticketService = ticketService;
            _context = appDbContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [Authorize(Policy = "Permissions.Tickets.View")]
        public async Task<IActionResult> Get()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpPost]
        [Authorize(Policy = "Permissions.Tickets.Create")]
        public async Task<IActionResult> Post([FromForm] TicketCreateDto ticketCreateDto)
        {
            var validationResult = await _createValidator.ValidateAsync(ticketCreateDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            await _ticketService.CreateTicketAsync(ticketCreateDto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Permissions.Tickets.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return Ok();
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Permissions.Tickets.Update")]
        public async Task<IActionResult> Put(int id, [FromForm] TicketUpdateDto ticketUpdateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(ticketUpdateDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            await _ticketService.UpdateTicketAsync(id, ticketUpdateDto);
            return Ok();
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Permissions.Tickets.View")]
        public async Task<IActionResult> Get(int id)
        {
            var ticketDto = await _ticketService.GetTicketByIdAsync(id);
            return Ok(ticketDto);
        }
    }
}

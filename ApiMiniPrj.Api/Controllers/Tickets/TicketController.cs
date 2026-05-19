using ApiMiniPrj.Application.DTOs.Tickets;
using ApiMiniPrj.Application.Interfaces.Common;
using ApiMiniPrj.Application.Interfaces.Tickets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMiniPrj.Api.Controllers.Tickets
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IAppDbContext _context;

        public TicketController(ITicketService ticketService, IAppDbContext appDbContext)
        {
            _ticketService = ticketService;
            _context = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] TicketCreateDto ticketCreateDto)
        {
            await _ticketService.CreateTicketAsync(ticketCreateDto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] TicketUpdateDto ticketUpdateDto)
        {
            await _ticketService.UpdateTicketAsync(id, ticketUpdateDto);
            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var ticketDto = await _ticketService.GetTicketByIdAsync(id);
            return Ok(ticketDto);
        }
    }
}

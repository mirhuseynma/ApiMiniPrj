using ApiMiniPrj.Application.DTOs.Tickets;
using ApiMiniPrj.Application.Interfaces.Tickets;
using ApiMiniPrj.Domain.Models.Tickets;
using ApiMiniPrj.Persistence.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiMiniPrj.Persistence.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TicketService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateTicketAsync(TicketCreateDto createTicketDto)
        {
            var ticket = _mapper.Map<Ticket>(createTicketDto);

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTicketAsync(int ticketId, TicketUpdateDto updateTicketDto)
        {
            var ticket = await GetTicketEntityAsync(ticketId);

            _mapper.Map(updateTicketDto, ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(int ticketId)
        {
            var ticket = await GetTicketEntityAsync(ticketId);

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<GetTicketDto> GetTicketByIdAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketId && !t.IsDeleted);

            if (ticket is null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }

            return _mapper.Map<GetTicketDto>(ticket);
        }

        public async Task<IEnumerable<GetTicketDto>> GetAllTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Where(t => !t.IsDeleted)
                .Include(t => t.Event)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetTicketDto>>(tickets);
        }

        private async Task<Ticket> GetTicketEntityAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId && !t.IsDeleted);

            if (ticket is null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }

            return ticket;
        }
    }
}

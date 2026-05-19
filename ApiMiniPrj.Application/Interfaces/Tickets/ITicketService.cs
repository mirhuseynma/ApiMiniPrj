using ApiMiniPrj.Application.DTOs.Tickets;

namespace ApiMiniPrj.Application.Interfaces.Tickets
{
    public interface ITicketService
    {
        Task CreateTicketAsync(TicketCreateDto createTicketDto);
        Task UpdateTicketAsync(int ticketId, TicketUpdateDto updateTicketDto);
        Task DeleteTicketAsync(int ticketId);
        Task<GetTicketDto> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<GetTicketDto>> GetAllTicketsAsync();
    }
}

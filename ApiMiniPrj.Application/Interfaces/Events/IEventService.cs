
namespace ApiMiniPrj.Application.Interfaces.Events
{
    public interface IEventService
    {
        Task CreateEventAsync(EventCreateDto eventCreateDto);
        Task DeleteEventAsync(int eventId);
        Task UpdateEventAsync(int eventId, EventUpdateDto eventUpdateDto);
        Task<GetEventDto> GetEventById(int eventId);
        Task<List<GetEventDto>> GetAllEventsAsync();
        Task<GetEventDto> GetEventByTitle(string title);
        Task AddBannerImageAsync(int eventId, IFormFile bannerImage);
    }
}

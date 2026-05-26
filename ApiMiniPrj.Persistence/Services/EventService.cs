

namespace ApiMiniPrj.Persistence.Services
{
    public class EventService : IEventService
    {
        private readonly IAppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        

        public EventService(IAppDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public async Task CreateEventAsync(EventCreateDto eventCreateDto)
        {

            var organizer = _context.Organizers.FirstOrDefault(o => o.Id == eventCreateDto.OrganizerId) ?? throw new Exception("organizer not found");
            var eventEntity = _mapper.Map<Event>(eventCreateDto);

            if (eventCreateDto.BannerImage is not null)
            {
                eventEntity.BannerImageUrl = await _fileStorageService.SaveFileAsync(eventCreateDto.BannerImage, "events");
            }

            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var eventEntity = await GetEventEntityAsync(eventId);

            _fileStorageService.DeleteFile(eventEntity.BannerImageUrl, "events");
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(int eventId, EventUpdateDto eventUpdateDto)
        { 
            var eventEntity = await GetEventEntityAsync(eventId);

            _mapper.Map(eventUpdateDto, eventEntity);

            if (eventUpdateDto.BannerImage is not null)
            {
                _fileStorageService.DeleteFile(eventEntity.BannerImageUrl, "events");
                eventEntity.BannerImageUrl = await _fileStorageService.SaveFileAsync(eventUpdateDto.BannerImage, "events");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<GetEventDto> GetEventById(int eventId)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Tickets.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted);

            return eventEntity is null ? throw new KeyNotFoundException("Event not found.") : _mapper.Map<GetEventDto>(eventEntity);
        }

        public async Task<List<GetEventDto>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .Where(e => !e.IsDeleted)
                .Include(e => e.Organizer)
                .Include(e => e.Tickets.Where(t => !t.IsDeleted))
                .ToListAsync();
            

            return events is null ? throw new KeyNotFoundException("Events not found.") : _mapper.Map<List<GetEventDto>>(events);
        }

        public async Task<GetEventDto> GetEventByTitle(string title)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Tickets.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(e => e.Title == title && !e.IsDeleted);

            return eventEntity is null ? throw new KeyNotFoundException("Event not found.") : _mapper.Map<GetEventDto>(eventEntity);
        }

        public async Task AddBannerImageAsync(int eventId, IFormFile bannerImage)
        {
            
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted) ?? throw new KeyNotFoundException("Event not found.");
            if (eventEntity.BannerImageUrl is not null)
            {
                _fileStorageService.DeleteFile(eventEntity.BannerImageUrl, "events");
            }

            var bannerImageUrl = await _fileStorageService.SaveFileAsync(bannerImage, "events");
            eventEntity.BannerImageUrl = bannerImageUrl;

            await _context.SaveChangesAsync();
        }

        private async Task<Event> GetEventEntityAsync(int eventId)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Tickets.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted);

            return eventEntity is null ? throw new KeyNotFoundException("Event not found.") : eventEntity;
        }
    }
}

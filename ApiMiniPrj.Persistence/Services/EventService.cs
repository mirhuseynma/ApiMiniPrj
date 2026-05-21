

namespace ApiMiniPrj.Persistence.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly IValidator<EventCreateDto> _createValidator;
        private readonly IValidator<EventUpdateDto> _updateValidator;
        private readonly IValidator<EventBannerImageUploadDto> _bannerImageUploadValidator;
        

        public EventService(AppDbContext context, IFileStorageService fileStorageService, IMapper mapper, IValidator<EventCreateDto> createValidator, IValidator<EventUpdateDto> updateValidator, IValidator<EventBannerImageUploadDto> bannerImageUploadValidator)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _bannerImageUploadValidator = bannerImageUploadValidator;
        }

        public async Task CreateEventAsync(EventCreateDto eventCreateDto)
        {
            var validationResult = await _createValidator.ValidateAsync(eventCreateDto);
            if(!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var errorMessage = $"Validation failed: {string.Join(", ", errors)}";
                throw new Exception(errorMessage);
            }

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

            _fileStorageService.DeleteFile(eventEntity.BannerImageUrl);
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(int eventId, EventUpdateDto eventUpdateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(eventUpdateDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var errorMessage = $"Validation failed: {string.Join(", ", errors)}";
                throw new Exception(errorMessage);
            }

            var eventEntity = await GetEventEntityAsync(eventId);

            _mapper.Map(eventUpdateDto, eventEntity);

            if (eventUpdateDto.BannerImage is not null)
            {
                _fileStorageService.DeleteFile(eventEntity.BannerImageUrl);
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

            if (eventEntity is null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            return _mapper.Map<GetEventDto>(eventEntity);
        }

        public async Task<List<GetEventDto>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .Where(e => !e.IsDeleted)
                .Include(e => e.Organizer)
                .Include(e => e.Tickets.Where(t => !t.IsDeleted))
                .ToListAsync();

            return _mapper.Map<List<GetEventDto>>(events);
        }

        public async Task<GetEventDto> GetEventByTitle(string title)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Tickets.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(e => e.Title == title && !e.IsDeleted);

            if (eventEntity is null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            return _mapper.Map<GetEventDto>(eventEntity);
        }

        public async Task AddBannerImageAsync(int eventId, IFormFile bannerImage)
        {
            var validationResult = await _bannerImageUploadValidator.ValidateAsync(new EventBannerImageUploadDto { BannerImage = bannerImage });

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var errorMessage = $"Validation failed: {string.Join(", ", errors)}";
                throw new Exception(errorMessage);
            }

            

            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted);

            if (eventEntity is null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            if (eventEntity.BannerImageUrl is not null)
            {
                _fileStorageService.DeleteFile(eventEntity.BannerImageUrl);
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

            if (eventEntity is null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            return eventEntity;
        }
    }
}

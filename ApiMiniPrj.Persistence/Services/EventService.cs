using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Application.Interfaces.Common;
using ApiMiniPrj.Application.Interfaces.Events;
using ApiMiniPrj.Domain.Models.Events;
using ApiMiniPrj.Persistence.Context;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiMiniPrj.Persistence.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly IValidator<EventCreateDto> _createValidator;

        public EventService(AppDbContext context, IFileStorageService fileStorageService, IMapper mapper, IValidator<EventCreateDto> createValidator)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        public async Task CreateEventAsync(EventCreateDto eventCreateDto)
        {
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
            if (bannerImage.Length == 0)
            {
                throw new ArgumentException("Banner image is required.", nameof(bannerImage));
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

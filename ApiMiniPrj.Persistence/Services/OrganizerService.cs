using ApiMiniPrj.Application.DTOs.Organizers;
using ApiMiniPrj.Application.Interfaces.Common;
using ApiMiniPrj.Application.Interfaces.Organizers;
using ApiMiniPrj.Domain.Models.Organizers;
using ApiMiniPrj.Persistence.Context;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiMiniPrj.Persistence.Services
{
    public class OrganizerService : IOrganizerService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public OrganizerService(AppDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public async Task CreateOrganizerAsync(OrganizerCreateDto createOrganizerDto)
        {
            var organizer = _mapper.Map<Organizer>(createOrganizerDto);

            if (createOrganizerDto.Logo is not null)
            {
                organizer.LogoUrl = await _fileStorageService.SaveFileAsync(createOrganizerDto.Logo, "organizers");
            }

            await _context.Organizers.AddAsync(organizer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrganizerAsync(int id)
        {
            var organizer = await GetOrganizerEntityAsync(id);

            _fileStorageService.DeleteFile(organizer.LogoUrl);
            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrganizerAsync(int id, OrganizerUpdateDto updateOrganizerDto)
        {
            var organizer = await GetOrganizerEntityAsync(id);

            _mapper.Map(updateOrganizerDto, organizer);

            if (updateOrganizerDto.Logo is not null)
            {
                _fileStorageService.DeleteFile(organizer.LogoUrl);
                organizer.LogoUrl = await _fileStorageService.SaveFileAsync(updateOrganizerDto.Logo, "organizers");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<GetOrganizerDto> GetOrganizerByIdAsync(int id)
        {
            var organizer = await _context.Organizers
                .Include(o => o.Events.Where(e => !e.IsDeleted))
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            if (organizer is null)
            {
                throw new KeyNotFoundException("Organizer not found.");
            }

            return _mapper.Map<GetOrganizerDto>(organizer);
        }

        public async Task<IEnumerable<GetOrganizerDto>> GetAllOrganizersAsync()
        {
            var organizers = await _context.Organizers
                .Where(o => !o.IsDeleted)
                .Include(o => o.Events.Where(e => !e.IsDeleted))
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOrganizerDto>>(organizers);
        }

        public async Task OrganizerUploadLogo(int id, IFormFile logo)
        {
            if (logo.Length == 0)
            {
                throw new ArgumentException("Logo is required.", nameof(logo));
            }

            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            if (organizer is null)
            {
                throw new KeyNotFoundException("Organizer not found.");
            }

            if (organizer.LogoUrl is not null)
            {
                _fileStorageService.DeleteFile(organizer.LogoUrl);
            }

            var logoUrl = await _fileStorageService.SaveFileAsync(logo, "organizers");
            organizer.LogoUrl = logoUrl;

            await _context.SaveChangesAsync();
        }

        private async Task<Organizer> GetOrganizerEntityAsync(int id)
        {
            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            if (organizer is null)
            {
                throw new KeyNotFoundException("Organizer not found.");
            }

            return organizer;
        }

        
    }
}


namespace ApiMiniPrj.Persistence.Services
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IAppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public OrganizerService(IAppDbContext context, IFileStorageService fileStorageService, IMapper mapper)
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

            _fileStorageService.DeleteFile(organizer.LogoUrl, "organizers");
            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrganizerAsync(int id, OrganizerUpdateDto updateOrganizerDto)
        {
            var organizer = await GetOrganizerEntityAsync(id);

            _mapper.Map(updateOrganizerDto, organizer);

            if (updateOrganizerDto.Logo is not null)
            {
                _fileStorageService.DeleteFile(organizer.LogoUrl, "organizers");
                organizer.LogoUrl = await _fileStorageService.SaveFileAsync(updateOrganizerDto.Logo, "organizers");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<GetOrganizerDto> GetOrganizerByIdAsync(int id)
        {
            var organizer = await _context.Organizers
                .Include(o => o.Events.Where(e => !e.IsDeleted))
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            return organizer is null ? throw new NotFoundException("Organizer not found.") : _mapper.Map<GetOrganizerDto>(organizer);
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
                throw new BadRequestException("Logo is required.");
            }

            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted) ?? throw new NotFoundException("Organizer not found.");
            if (organizer.LogoUrl is not null)
            {
                _fileStorageService.DeleteFile(organizer.LogoUrl, "organizers");
            }

            var logoUrl = await _fileStorageService.SaveFileAsync(logo, "organizers");
            organizer.LogoUrl = logoUrl;

            await _context.SaveChangesAsync();
        }

        private async Task<Organizer> GetOrganizerEntityAsync(int id)
        {
            var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            return organizer is null ? throw new NotFoundException("Organizer not found.") : organizer;
        }


    }
}

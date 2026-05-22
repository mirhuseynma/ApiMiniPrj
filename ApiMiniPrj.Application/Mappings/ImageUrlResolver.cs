namespace ApiMiniPrj.Application.Mappings
{
    public abstract class ImageUrlResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _folderName;

        protected ImageUrlResolver(IHttpContextAccessor httpContextAccessor, string folderName)
        {
            _httpContextAccessor = httpContextAccessor;
            _folderName = folderName;
        }

        protected string? BuildUrl(string? sourceMember)
        {
            if (string.IsNullOrWhiteSpace(sourceMember))
            {
                return null;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var fileName = Path.GetFileName(sourceMember);

            if (httpContext is null)
            {
                return $"/uploads/{_folderName}/{fileName}";
            }

            var request = httpContext.Request;

            return $"{request.Scheme}://{request.Host}/uploads/{_folderName}/{fileName}";
        }
    }

    public class EventImageUrlResolver<TDestination> : ImageUrlResolver, IMemberValueResolver<Event, TDestination, string?, string?>
    {
        public EventImageUrlResolver(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, "events")
        {
        }

        public string? Resolve(Event source, TDestination destination, string? sourceMember, string? destMember, ResolutionContext context)
        {
            return BuildUrl(sourceMember);
        }
    }

    public class OrganizerLogoUrlResolver<TDestination> : ImageUrlResolver, IMemberValueResolver<Organizer, TDestination, string?, string?>
    {
        public OrganizerLogoUrlResolver(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, "organizers")
        {
        }

        public string? Resolve(Organizer source, TDestination destination, string? sourceMember, string? destMember, ResolutionContext context)
        {
            return BuildUrl(sourceMember);
        }
    }
}

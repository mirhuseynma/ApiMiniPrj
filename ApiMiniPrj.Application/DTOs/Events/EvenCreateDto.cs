using Microsoft.AspNetCore.Http;

namespace ApiMiniPrj.Application.DTOs.Events
{
    public class EventCreateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public int OrganizerId { get; set; }
        public IFormFile? BannerImage { get; set; }
    }
}

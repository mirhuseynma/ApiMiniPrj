using Microsoft.AspNetCore.Http;

namespace ApiMiniPrj.Application.DTOs.Organizers
{
    public class OrganizerUpdateDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? Logo { get; set; }
    }
}

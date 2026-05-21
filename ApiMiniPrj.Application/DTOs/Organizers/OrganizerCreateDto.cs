
namespace ApiMiniPrj.Application.DTOs.Organizers
{
    public class OrganizerCreateDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public IFormFile? Logo { get; set; }
    }
}

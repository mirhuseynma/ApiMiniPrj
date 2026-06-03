using ApiMiniPrj.Application.DTOs.Organizers;

namespace ApiMiniPrj.Mvc.Models.Organizers
{
    public class OrganizerIndexVM
    {
        public List<GetOrganizerDto> Organizers { get; set; } = [];
    }

    public class OrganizerCreateVM
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public IFormFile? Logo { get; set; }
    }

    public class OrganizerUpdateVM
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LogoUrl { get; set; }
        public IFormFile? Logo { get; set; }
    }
}

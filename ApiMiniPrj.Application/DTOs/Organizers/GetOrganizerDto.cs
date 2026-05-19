namespace ApiMiniPrj.Application.DTOs.Organizers
{
    public class GetOrganizerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? LogoUrl { get; set; } 
        public List<EventsForOrganizerDto> Events { get; set; } = [];
    }

    public class EventsForOrganizerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
    }
}

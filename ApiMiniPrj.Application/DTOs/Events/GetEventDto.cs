namespace ApiMiniPrj.Application.DTOs.Events
{
    public class GetEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
        public OrganizerForEventDto ?Organizer { get; set; }
        public List<TicketsForEventDto> Tickets { get; set; } = [];

    }

    public class TicketsForEventDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class OrganizerForEventDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }

}

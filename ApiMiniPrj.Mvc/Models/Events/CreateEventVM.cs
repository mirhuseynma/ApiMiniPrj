namespace ApiMiniPrj.Mvc.Models.Events
{
    public class CreateEventVM
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now.AddDays(1);
        public string Location { get; set; } = string.Empty;
        public int OrganizerId { get; set; }
        public IFormFile? BannerImage { get; set; }
        public List<EventOrganizerOptionVM> Organizers { get; set; } = [];
    }

    public class UpdateEventVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string? Location { get; set; }
        public int? OrganizerId { get; set; }
        public string? BannerImageUrl { get; set; }
        public IFormFile? BannerImage { get; set; }
        public List<EventOrganizerOptionVM> Organizers { get; set; } = [];
    }

    public class EventOrganizerOptionVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}


namespace ApiMiniPrj.Domain.Models.Organizers
{
    public class Organizer : AuditableEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? LogoUrl { get; set; }

        // relation with Events
        public ICollection<Event> Events { get; set; } = [];


    }
}

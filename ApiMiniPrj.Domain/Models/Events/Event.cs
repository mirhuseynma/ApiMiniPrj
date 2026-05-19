using ApiMiniPrj.Domain.Models.Common;
using ApiMiniPrj.Domain.Models.Organizers;
using ApiMiniPrj.Domain.Models.Tickets;

namespace ApiMiniPrj.Domain.Models.Events
{
    public class Event : AuditableEntity
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public string ?BannerImageUrl { get; set; }

        // relation with Organizers
        public int OrganizerId { get; set; }
        public Organizer ?Organizer { get; set; }

        // relation with Tickets
        public ICollection<Ticket> Tickets { get; set; } = [];
    }
}

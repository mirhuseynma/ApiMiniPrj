
namespace ApiMiniPrj.Domain.Models.Tickets
{
    public class Ticket : AuditableEntity
    {
        public TicketTypeEnum Type { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Price { get; set; }
        public bool IsAvaiable { get; set; } = true;


        // relation with Event
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}

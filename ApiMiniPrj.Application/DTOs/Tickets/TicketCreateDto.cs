using ApiMiniPrj.Domain.Enums;

namespace ApiMiniPrj.Application.DTOs.Tickets
{
    public class TicketCreateDto
    {
        public TicketTypeEnum Type { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsAvaiable { get; set; }
        public int EventId { get; set; }
    }
}

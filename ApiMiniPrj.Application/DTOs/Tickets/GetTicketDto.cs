

namespace ApiMiniPrj.Application.DTOs.Tickets
{
    public class GetTicketDto
    {
        public int Id { get; set; }
        public TicketTypeEnum Type { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsAvaiable { get; set; }
        public EventForTicketDto? Event { get; set; }
    }

    public class EventForTicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
    }
}

using ApiMiniPrj.Application.DTOs.Tickets;
using ApiMiniPrj.Domain.Enums;

namespace ApiMiniPrj.Mvc.Models.Tickets
{
    public class TicketIndexVM
    {
        public List<GetTicketDto> Tickets { get; set; } = [];
    }

    public class TicketCreateVM
    {
        public TicketTypeEnum Type { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsAvaiable { get; set; } = true;
        public int EventId { get; set; }
        public List<TicketEventOptionVM> Events { get; set; } = [];
    }

    public class TicketUpdateVM
    {
        public int Id { get; set; }
        public TicketTypeEnum? Type { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvaiable { get; set; }
        public int? EventId { get; set; }
        public List<TicketEventOptionVM> Events { get; set; } = [];
    }

    public class TicketEventOptionVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}

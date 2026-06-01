using ApiMiniPrj.Application.DTOs.Events;

namespace ApiMiniPrj.Mvc.Models.Events
{
    public class GetEventsVM
    {
        public List<GetEventDto> Events { get; set; } = [];
    }
}

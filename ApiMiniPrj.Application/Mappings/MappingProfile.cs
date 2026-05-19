using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Application.DTOs.Organizers;
using ApiMiniPrj.Application.DTOs.Tickets;
using ApiMiniPrj.Domain.Models.Events;
using ApiMiniPrj.Domain.Models.Organizers;
using ApiMiniPrj.Domain.Models.Tickets;
using AutoMapper;

namespace ApiMiniPrj.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventCreateDto, Event>()
                .ForMember(dest => dest.BannerImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            CreateMap<EventUpdateDto, Event>()
                .ForMember(dest => dest.BannerImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, sourceMember) => sourceMember is not null));

            CreateMap<Event, GetEventDto>();
            CreateMap<Event, EventsForOrganizerDto>();
            CreateMap<Event, EventForTicketDto>();

            CreateMap<OrganizerCreateDto, Organizer>()
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore());

            CreateMap<OrganizerUpdateDto, Organizer>()
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, sourceMember) => sourceMember is not null));

            CreateMap<Organizer, GetOrganizerDto>();
            CreateMap<Organizer, OrganizerForEventDto>();

            CreateMap<TicketCreateDto, Ticket>()
                .ForMember(dest => dest.Event, opt => opt.Ignore());

            CreateMap<TicketUpdateDto, Ticket>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, sourceMember) => sourceMember is not null));

            CreateMap<Ticket, GetTicketDto>();
            CreateMap<Ticket, TicketsForEventDto>()
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvaiable));
        }
    }
}

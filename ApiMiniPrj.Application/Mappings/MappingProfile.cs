

using AutoMapper;
using ApiMiniPrj.Application.DTOs.Auth;
using ApiMiniPrj.Application.DTOs.Events;
using ApiMiniPrj.Application.DTOs.Organizers;
using ApiMiniPrj.Application.DTOs.Tickets;
using ApiMiniPrj.Application.DTOs.Users;
using ApiMiniPrj.Domain.Models;
using ApiMiniPrj.Domain.Models.Users;

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

            // Auth DTOs
            CreateMap<RegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .AfterMap((src, dest) =>
                {
                    var nameParts = src.FullName.Split(' ');
                    dest.FirstName = nameParts[0];
                    dest.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                });

            // User DTOs
            CreateMap<AppUser, UserGetDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => (src.FirstName + " " + src.LastName).Trim()));

            CreateMap<UserUpdateDto, AppUser>()
                .AfterMap((src, dest) =>
                {
                    if (src.FullName != null)
                    {
                        var nameParts = src.FullName.Split(' ');
                        dest.FirstName = nameParts[0];
                        dest.LastName = nameParts.Length > 1 ? nameParts[1] : null;
                    }
                })
                .ForMember(dest => dest.UserName, opt => opt.Condition(src => src.UserName != null))
                .ForMember(dest => dest.Email, opt => opt.Condition(src => src.Email != null))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());  // Password is handled separately via UserManager


        }
    }
}

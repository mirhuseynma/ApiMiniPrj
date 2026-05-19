using ApiMiniPrj.Application.Interfaces.Common;
using ApiMiniPrj.Application.Interfaces.Events;
using ApiMiniPrj.Application.Interfaces.Organizers;
using ApiMiniPrj.Application.Interfaces.Tickets;
using ApiMiniPrj.Persistence.Context;
using ApiMiniPrj.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiMiniPrj.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IOrganizerService, OrganizerService>();
            services.AddScoped<ITicketService, TicketService>();

            return services;
        }
    }
}

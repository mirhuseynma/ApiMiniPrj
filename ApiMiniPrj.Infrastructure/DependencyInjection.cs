using ApiMiniPrj.Application.Interfaces.Common;
using ApiMiniPrj.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiMiniPrj.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IFileStorageService, FileStorageService>();
            return services;
        }
    }
}

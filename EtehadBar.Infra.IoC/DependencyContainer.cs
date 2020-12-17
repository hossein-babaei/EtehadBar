using EtehadBar.Domain.Interfaces;
using EtehadBar.Infra.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EtehadBar.Infra.IoC
{
    public static class DependencyContainer
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminThemeRepository, AdminThemeRepository>();

            return services;
        }
    }
}

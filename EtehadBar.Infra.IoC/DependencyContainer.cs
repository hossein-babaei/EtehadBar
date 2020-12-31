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
            services.AddScoped<ICalendarRepository, CalendarRepository>();
            services.AddScoped<IConfigRepository, ConfigRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<ICostRepository, CostRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDefinitionRepository, DefinitionRepository>();
            services.AddScoped<ILoadFactorRepository, LoadFactorRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IShippingFeeRepository, ShippingFeeRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            return services;
        }
    }
}

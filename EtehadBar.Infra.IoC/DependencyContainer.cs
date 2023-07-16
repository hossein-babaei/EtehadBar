using EtehadBar.Domain.Interfaces;
using EtehadBar.Infra.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EtehadBar.Infra.IoC
{
    public static class DependencyContainer
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
            services.AddScoped<IAccountBookRepository, AccountBookRepository>();
            services.AddScoped<IAdminThemeRepository, AdminThemeRepository>();
            services.AddScoped<IBankAccountBookRepository, BankAccountBookRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ICalendarRepository, CalendarRepository>();
            services.AddScoped<IConfigRepository, ConfigRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<ICostRepository, CostRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerPeriodicBalanceSummaryRepository, CustomerPeriodicBalanceSummaryRepository>();
            services.AddScoped<IDefinitionRepository, DefinitionRepository>();
            services.AddScoped<ILoadFactorRepository, LoadFactorRepository>();
            services.AddScoped<IShippingFeeRepository, ShippingFeeRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IShippingFeeLoadTypeRepository, ShippingFeeLoadTypeRepository>();
            services.AddScoped<ILoadRoutesRepository, LoadRoutesRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IMehrcomParsCategoryRepository, MehrcomParsCategoryRepository>();
            services.AddScoped<IFreeLoadFactorRepository, FreeLoadFactorRepository>();
            services.AddScoped<ITurnoverRepository, TurnoverRepository>();
            services.AddScoped<ITurnoverProfileRepository, TurnoverProfileRepository>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IVehicleBalanceRepository, VehicleBalanceRepository>();
            services.AddScoped<ICustomerFactorRepository, CustomerFactorRepository>();

            return services;
        }
    }
}

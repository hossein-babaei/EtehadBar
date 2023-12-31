using EtehadBar.Domain.Models;
using EtehadBar.Domain.Models.LoadFactorCreator;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EtehadBar.Infra.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccountBook> AccountBook { get; set; }
        public DbSet<AdminTheme> AdminTheme { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<BankAccountBook> BankAccountBook { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<Calendar> Calendar { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<Cost> Cost { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerFactor> CustomerFactor { get; set; }
        public DbSet<CustomerIncome> CustomerIncome { get; set; }
        public DbSet<CustomerPeriodicBalanceAddon> CustomerPeriodicBalanceAddon { get; set; }
        public DbSet<CustomerPeriodicBalanceSummary> CustomerPeriodicBalanceSummary { get; set; }
        public DbSet<Definition> Definition { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<FreeLoadFactor> FreeLoadFactor { get; set; }
        public DbSet<LoadFactor> LoadFactor { get; set; }
        public DbSet<LoadFactorNovin> LoadFactorNovin { get; set; }
        public DbSet<LoadRoutes> LoadRoute { get; set; }
        public DbSet<SaipaPlascoLoadFactor> SaipaPlascoLoadFactor { get; set; }
        public DbSet<SaipaPressLoadFactor> SaipaPressLoadFactor { get; set; }
        public DbSet<SazehGostarLoadFactor> SazehGostarLoadFactor { get; set; }
        public DbSet<MehrcomParsLoadFactor> MehrcomParsLoadFactor { get; set; }
        public DbSet<MehrcomParsCategory> MehrcomParsCategory { get; set; }
        public DbSet<OtherCost> OtherCost { get; set; }
        public DbSet<ShippingFeeLoadType> ShippingFeeLoadType { get; set; }
        public DbSet<ShippingFee> ShippingFee { get; set; }
        public DbSet<StaticRouteFee> StaticRouteFee { get; set; }
        public DbSet<UploadedFiles> UploadedFiles { get; set; }
        public DbSet<Turnover> Turnover { get; set; }
        public DbSet<TurnoverProfile> TurnoverProfile { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleBankAccount> VehicleBankAccount { get; set; }
        public DbSet<VehicleBalance> VehicleBalance { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");

            //modelBuilder.Entity<LoadFactor>()
            //    .Property(a => a.Counter)
            //    .UseIdentityColumn(seed: 1, increment: 1);

            //modelBuilder.Entity<LoadFactor>().Property(u => u.Counter).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<LoadFactor>().HasOne(a => a.AccountBook).WithMany(a => a.LoadFactors).OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<CustomerIncome>().HasOne(a => a.Contract).WithMany(a => a.Incomes).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ShippingFee>().HasOne(sf => sf.Origin).WithMany().HasForeignKey(sf => sf.OriginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ShippingFee>().HasOne(sf => sf.Destination).WithMany().HasForeignKey(sf => sf.DestinationId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LoadFactor>().HasOne(lf => lf.Origin).WithMany().HasForeignKey(lf => lf.OriginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<LoadFactor>().HasOne(lf => lf.Destination).WithMany().HasForeignKey(lf => lf.DestinationId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Customer>().HasData(
                new { Id = 1L, Name = "پلاسکو کار سایپا", CustomerType = Domain.CustomerType.SaipaPlasco, LoadFactorDeductions = 5d, HasLoadType = false, HasAddonTonnage = false, HasLoadSleep = false, Status = true, ActiveBank = 43L, RowId = "29f78114-f72a-427a-a3f1-8864e6eeb13c" },
                new { Id = 2L, Name = "سایپا پرس", CustomerType = Domain.CustomerType.SaipaPress, LoadFactorDeductions = 5d, HasLoadType = true, HasAddonTonnage = true, HasLoadSleep = false, Status = true, ActiveBank = 43L, RowId = "e1cbee6e-f7a1-4a84-a1c5-e740fb84fa7d" },
                new { Id = 3L, Name = "سازه گستر", CustomerType = Domain.CustomerType.SazehGostar, LoadFactorDeductions = 7.8d, HasLoadType = false, HasAddonTonnage = false, HasLoadSleep = false, Status = true, ActiveBank = 43L, RowId = "df204398-5c7c-4caf-98c0-0c9b9be54a6f" },
                new { Id = 4L, Name = "مهرکام پارس", CustomerType = Domain.CustomerType.MehrcomPars, LoadFactorDeductions = 5d, HasLoadType = true, HasAddonTonnage = true, HasLoadSleep = true, Status = true, ActiveBank = 45L, RowId = "e70bffab-fa42-4c66-8af8-d7090a6ccbea" });

            modelBuilder.Entity<ShippingFeeLoadType>().HasData(
                new ShippingFeeLoadType() { Id = -1L, Name = "کالا", RowId = "e015d881-cf4f-40b2-bf83-0a115bae3179" });

            modelBuilder.Entity<Config>().HasData(
                new Config() { Id = 1, VAT = 9, WithholdingTax = 3, Year = PersianDateTime.Now.ToString("yyyy"), RowId = "8bd8d4c9-7595-4b03-95c7-91ab91046965" });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

                builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }

            base.OnConfiguring(builder);
        }
    }
}

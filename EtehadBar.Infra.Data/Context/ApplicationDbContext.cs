using EtehadBar.Domain.Models;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

        public DbSet<AdminTheme> AdminTheme { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Calendar> Calendar { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<Cost> Cost { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerIncome> CustomerIncome { get; set; }
        public DbSet<Definition> Definition { get; set; }
        public DbSet<LoadFactor> LoadFactor { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<SaipaPressLoadFactor> SaipaPressLoadFactor { get; set; }
        public DbSet<SazehGostarLoadFactor> SazehGostarLoadFactor { get; set; }
        public DbSet<ShippingFee> ShippingFee { get; set; }
        public DbSet<UploadedFiles> UploadedFiles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<LoadFactor>()
                .Property(a => a.Counter)
                .UseIdentityColumn(seed: 1, increment: 1);

            modelBuilder.Entity<LoadFactor>().Property(u => u.Counter).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<Customer>().HasData(
                new { Id = 1, Name = "پلاسکو کار سایپا", CustomerType = Domain.CustomerType.SaipaPlasco, Status = true },
                new { Id = 2, Name = "سایپا پرس", CustomerType = Domain.CustomerType.SaipaPress, Status = true },
                new { Id = 3, Name = "سازه گستر", CustomerType = Domain.CustomerType.SazehGostar, Status = true });

            modelBuilder.Entity<Config>().HasData(
                new Config() { Id = 1, VAT = 9, LoadFactorDeductions = 5, WithholdingTax = 3, Year = PersianDateTime.Now.ToString("yyyy") });
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

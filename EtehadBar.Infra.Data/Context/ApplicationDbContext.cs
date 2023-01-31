using EtehadBar.Domain.Models;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;

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
        public DbSet<LoadRoutes> LoadRoute { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<SaipaPressLoadFactor> SaipaPressLoadFactor { get; set; }
        public DbSet<SazehGostarLoadFactor> SazehGostarLoadFactor { get; set; }
        public DbSet<ShippingFeeLoadType> ShippingFeeLoadType { get; set; }
        public DbSet<ShippingFee> ShippingFee { get; set; }
        public DbSet<UploadedFiles> UploadedFiles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");

            //modelBuilder.Entity<LoadFactor>()
            //    .Property(a => a.Counter)
            //    .UseIdentityColumn(seed: 1, increment: 1);

            //modelBuilder.Entity<LoadFactor>().Property(u => u.Counter).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<ShippingFee>().HasOne(sf => sf.Origin).WithMany().HasForeignKey(sf => sf.OriginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ShippingFee>().HasOne(sf => sf.Destination).WithMany().HasForeignKey(sf => sf.DestinationId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LoadFactor>().HasOne(lf => lf.Origin).WithMany().HasForeignKey(lf => lf.OriginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<LoadFactor>().HasOne(lf => lf.Destination).WithMany().HasForeignKey(lf => lf.DestinationId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Customer>().HasData(
                new { Id = 1L, Name = "پلاسکو کار سایپا", CustomerType = Domain.CustomerType.SaipaPlasco, HasLoadType = false, HasAddonTonnage = false, Status = true, RowId = "29f78114-f72a-427a-a3f1-8864e6eeb13c" },
                new { Id = 2L, Name = "سایپا پرس", CustomerType = Domain.CustomerType.SaipaPress, HasLoadType = true, HasAddonTonnage = true, Status = true, RowId = "e1cbee6e-f7a1-4a84-a1c5-e740fb84fa7d" },
                new { Id = 3L, Name = "سازه گستر", CustomerType = Domain.CustomerType.SazehGostar, HasLoadType = false, HasAddonTonnage = false, Status = true, RowId = "df204398-5c7c-4caf-98c0-0c9b9be54a6f" },
                new { Id = 4L, Name = "مهرکام پارس", CustomerType = Domain.CustomerType.MehrcomPars, HasLoadType = false, HasAddonTonnage = false, Status = true, RowId = "e70bffab-fa42-4c66-8af8-d7090a6ccbea" });

            modelBuilder.Entity<ShippingFeeLoadType>().HasData(
                new ShippingFeeLoadType() { Id = -1L, Name = "کالا", RowId = "e015d881-cf4f-40b2-bf83-0a115bae3179" });

            modelBuilder.Entity<Config>().HasData(
                new Config() { Id = 1, VAT = 9, LoadFactorDeductions = 5, WithholdingTax = 3, Year = PersianDateTime.Now.ToString("yyyy"), RowId = "8bd8d4c9-7595-4b03-95c7-91ab91046965" });
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

using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using MD.PersianDateTime.Standard;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class LoadFactorRepository : ILoadFactorRepository
    {
        private readonly ApplicationDbContext db;
        public LoadFactorRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<LoadFactor> LoadFactors()
        {
            return db.LoadFactor.AsQueryable();
        }

        public void Create(LoadFactor obj)
        {
            db.Add(obj);
        }

        public void Delete(LoadFactor obj)
        {
            db.Remove(obj);
        }

        public async Task<LoadFactor> Get(string id)
        {
            return await db.LoadFactor.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(LoadFactor obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<LoadFactor> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }

        public void CreateSaipaPress(SaipaPressLoadFactor obj)
        {
            db.Add(obj);
        }

        public void CreateSazehGostar(SazehGostarLoadFactor obj)
        {
            db.Add(obj);
        }

        public void UpdateSaipaPress(SaipaPressLoadFactor obj)
        {
            db.Update(obj);
        }

        public void UpdateSazehGostar(SazehGostarLoadFactor obj)
        {
            db.Update(obj);
        }

        public async Task<ESaipaPlascoLoadFactorVM> GetSaipaPlascoLoadFactor(string loadFactorId)
        {
            var loadFactor = await db.LoadFactor.AsNoTracking().SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new ESaipaPlascoLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new ESaipaPlascoLoadFactorVM
            {
                Id = loadFactor.Id,
                CalendarId = loadFactor.CalendarId,
                ContractId = loadFactor.ContractId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                DriverId = loadFactor.DriverId,
                ExitNumber = loadFactor.ExitNumber,
                LoadNumber = loadFactor.LoadNumber,
                LoadNumberGov = loadFactor.LoadNumberGov,
                ShippingFeeId = loadFactor.ShippingFeeId,
                VehicleId = loadFactor.VehicleId
            };
        }

        public async Task<ESaipaPressLoadFactorVM> GetSaipaPressLoadFactor(string loadFactorId)
        {
            var loadFactor = await db.LoadFactor.AsNoTracking().SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new ESaipaPressLoadFactorVM();

            var saipaPressLoadFactor = loadFactor.SaipaPressLoadFactor;
            if (saipaPressLoadFactor == null) return new ESaipaPressLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new ESaipaPressLoadFactorVM
            {
                Id = loadFactor.Id,
                CalendarId = loadFactor.CalendarId,
                ContractId = loadFactor.ContractId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                DriverId = loadFactor.DriverId,
                ExitNumber = loadFactor.ExitNumber,
                LoadNumber = loadFactor.LoadNumber,
                LoadNumberGov = loadFactor.LoadNumberGov,
                ShippingFeeId = loadFactor.ShippingFeeId,
                VehicleId = loadFactor.VehicleId,
                EntryNumber = saipaPressLoadFactor.EntryNumber,
                LoadType = saipaPressLoadFactor.LoadType,
                RelationId = saipaPressLoadFactor.Id
            };
        }

        public async Task<ESazehGostarLoadFactorVM> GetSazehGostarLoadFactor(string loadFactorId)
        {
            var loadFactor = await db.LoadFactor.AsNoTracking().SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new ESazehGostarLoadFactorVM();

            var sazehGostarLoadFactor = loadFactor.SazehGostarLoadFactor;
            if (loadFactor == null) return new ESazehGostarLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new ESazehGostarLoadFactorVM
            {
                Id = loadFactor.Id,
                CalendarId = loadFactor.CalendarId,
                ContractId = loadFactor.ContractId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                DriverId = loadFactor.DriverId,
                ExitNumber = loadFactor.ExitNumber,
                LoadNumber = loadFactor.LoadNumber,
                LoadNumberGov = loadFactor.LoadNumberGov,
                ShippingFeeId = loadFactor.ShippingFeeId,
                VehicleId = loadFactor.VehicleId,
                RelationId = sazehGostarLoadFactor.Id,
                Certain = sazehGostarLoadFactor.Certain,
                Count = sazehGostarLoadFactor.Count,
                Description = sazehGostarLoadFactor.Description,
                DetailedCostCenter = sazehGostarLoadFactor.DetailedCostCenter,
                Nature = sazehGostarLoadFactor.Nature,
                RegisterCode = sazehGostarLoadFactor.RegisterCode,
                Status = sazehGostarLoadFactor.Status
            };
        }
    }
}

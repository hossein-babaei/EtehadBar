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

        public void DeleteSaipaPress(SaipaPressLoadFactor obj)
        {
            db.Remove(obj);
        }

        public void DeleteSazehGostar(SazehGostarLoadFactor obj)
        {
            db.Remove(obj);
        }

        public async Task<LoadFactor> Get(long id)
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

        public async Task<ESaipaPlascoLoadFactorVM> GetSaipaPlascoLoadFactor(long loadFactorId)
        {
            var loadFactor = await db.LoadFactor.SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new ESaipaPlascoLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new ESaipaPlascoLoadFactorVM
            {
                Sequence = loadFactor.SaipaPlascoLoadFactor.Sequence,
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
                Amount = loadFactor.Amount,
                DriverFee = loadFactor.DriverFee,
                AccountBookId = loadFactor.AccountBookId
            };
        }

        public async Task<ESaipaPressLoadFactorVM> GetSaipaPressLoadFactor(long loadFactorId)
        {
            var loadFactor = await db.LoadFactor.SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new ESaipaPressLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new ESaipaPressLoadFactorVM
            {
                Sequence = loadFactor.SaipaPressLoadFactor.Sequence,
                Id = loadFactor.Id,
                CalendarId = loadFactor.CalendarId,
                ContractId = loadFactor.ContractId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                DriverId = loadFactor.DriverId,
                ExitNumber = loadFactor.ExitNumber,
                LoadNumber = loadFactor.LoadNumber,
                ShippingFeeId = loadFactor.ShippingFeeId,
                VehicleId = loadFactor.VehicleId,
                EntryNumber = loadFactor.SaipaPressLoadFactor.EntryNumber,
                LoadType = loadFactor.SaipaPressLoadFactor.LoadType,
                RelationId = loadFactor.SaipaPressLoadFactor.Id,
                Amount = loadFactor.Amount,
                DriverFee = loadFactor.DriverFee,
                DriverTonnagePrice = loadFactor.DriverTonnagePrice,
                Tonnage = loadFactor.Tonnage,
                TonnagePrice = loadFactor.TonnagePrice,
                PressFloorType = loadFactor.SaipaPressLoadFactor.PressFloorType,
                AccountBookId = loadFactor.AccountBookId
            };
        }

        public async Task<ESazehGostarLoadFactorVM> GetSazehGostarLoadFactor(long loadFactorId)
        {
            var loadFactor = await db.LoadFactor.SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new ESazehGostarLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new ESazehGostarLoadFactorVM
            {
                Sequence = loadFactor.SazehGostarLoadFactor.Sequence,
                Id = loadFactor.Id,
                CalendarId = loadFactor.CalendarId,
                ContractId = loadFactor.ContractId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                DriverId = loadFactor.DriverId,
                ExitNumber = loadFactor.ExitNumber,
                LoadNumber = loadFactor.LoadNumber,
                ShippingFeeId = loadFactor.ShippingFeeId,
                VehicleId = loadFactor.VehicleId,
                RelationId = loadFactor.SazehGostarLoadFactor.Id,
                Certain = loadFactor.SazehGostarLoadFactor.Certain,
                Count = loadFactor.SazehGostarLoadFactor.Count,
                Description = loadFactor.SazehGostarLoadFactor.Description,
                DetailedCostCenter = loadFactor.SazehGostarLoadFactor.DetailedCostCenter,
                Nature = loadFactor.SazehGostarLoadFactor.Nature,
                RegisterCode = loadFactor.SazehGostarLoadFactor.RegisterCode,
                Amount = loadFactor.Amount,
                DriverFee = loadFactor.DriverFee,
                AccountBookId = loadFactor.AccountBookId
            };
        }

        public async Task<List<LoadFactor>> LoadFactors(long customerId, long? calendarId, long? accountBookId, long? driverId)
        {
            if (!calendarId.HasValue)
            {
                var data = from a in db.LoadFactor
                           join b in db.Contract on a.ContractId equals b.Id
                           where b.CustomerId.Equals(customerId)
                           select a;

                return await data.Where(a => accountBookId.HasValue ? a.AccountBookId.Equals(accountBookId.Value) : true &&
                driverId.HasValue ? a.DriverId.Equals(driverId.Value) : true).OrderBy(a => a.Date).ToListAsync();
            }
            else
            {
                var data = from a in db.LoadFactor
                           join b in db.Contract on a.ContractId equals b.Id
                           where a.CalendarId.Equals(calendarId.Value) && b.CustomerId.Equals(customerId)
                           select a;

                return await data.Where(a => accountBookId.HasValue ? a.AccountBookId.Equals(accountBookId.Value) : true &&
                driverId.HasValue ? a.DriverId.Equals(driverId.Value) : true).OrderBy(a => a.Date).ToListAsync();
            }
        }

        public async Task<long> GetBiggestSequenceInSaipaPlasco()
        {
            if (await db.SaipaPlascoLoadFactor.AsNoTracking().AnyAsync())
                return await db.SaipaPlascoLoadFactor.AsNoTracking().MaxAsync(a => a.Sequence);
            else 
                return 0;
        }

        public async Task<long> GetBiggestSequenceInSaipaPress()
        {
            if (await db.SaipaPressLoadFactor.AsNoTracking().AnyAsync())
                return await db.SaipaPressLoadFactor.AsNoTracking().MaxAsync(a => a.Sequence);
            else
                return 0;
        }

        public async Task<long> GetBiggestSequenceInSazehGostar()
        {
            if (await db.SazehGostarLoadFactor.AsNoTracking().AnyAsync())
                return await db.SazehGostarLoadFactor.AsNoTracking().MaxAsync(a => a.Sequence);
            else
                return 0;
        }

        public async Task<long> GetBiggestSequenceInMehrcomPars()
        {
            if (await db.MehrcomParsLoadFactor.AsNoTracking().AnyAsync())
                return await db.MehrcomParsLoadFactor.AsNoTracking().MaxAsync(a => a.Sequence);
            else
                return 0;
        }

        public async Task<bool> SequenceExistInSaipaPlasco(long id, long sequence)
        {
            return await db.SaipaPlascoLoadFactor.AsNoTracking().AnyAsync(a => a.Sequence.Equals(sequence) && !a.LoadFactorId.Equals(id));
        }

        public async Task<bool> SequenceExistInSaipaPress(long id, long sequence)
        {
            return await db.SaipaPressLoadFactor.AsNoTracking().AnyAsync(a => a.Sequence.Equals(sequence) && !a.LoadFactorId.Equals(id));
        }

        public async Task<bool> SequenceExistInSazehGostar(long id, long sequence)
        {
            return await db.SazehGostarLoadFactor.AsNoTracking().AnyAsync(a => a.Sequence.Equals(sequence) && !a.LoadFactorId.Equals(id));
        }

        public async Task<EMehrcomParsLoadFactorVM> GetMehrcomParsLoadFactor(long loadFactorId)
        {
            var loadFactor = await db.LoadFactor.SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new EMehrcomParsLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new EMehrcomParsLoadFactorVM
            {
                Id = loadFactor.Id,
                CalendarId = loadFactor.CalendarId,
                ContractId = loadFactor.ContractId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                DriverId = loadFactor.DriverId,
                LoadNumber = loadFactor.LoadNumber,
                ShippingFeeId = loadFactor.ShippingFeeId,
                VehicleId = loadFactor.VehicleId,
                Amount = loadFactor.Amount,
                DriverFee = loadFactor.DriverFee,
                AccountBookId = loadFactor.AccountBookId,
                Load = loadFactor.MehrcomParsLoadFactor.Load,
                LoadNumberGov= loadFactor.LoadNumberGov,
                LoadNumberGovReturn = loadFactor.MehrcomParsLoadFactor.LoadNumberGovReturn,
                Palette = loadFactor.MehrcomParsLoadFactor.Palette,
                Return = loadFactor.MehrcomParsLoadFactor.Return,
                WeighbridgePrice = loadFactor.MehrcomParsLoadFactor.WeighbridgePrice,
                CategoryId = loadFactor.MehrcomParsLoadFactor.CategoryId,
                DriverLoadSleepPrice = loadFactor.MehrcomParsLoadFactor.DriverLoadSleepPrice,
                DriverTonnagePrice = loadFactor.DriverTonnagePrice,
                Tonnage = loadFactor.Tonnage,
                LoadType = loadFactor.MehrcomParsLoadFactor.LoadType,
                TonnagePrice = loadFactor.TonnagePrice,
                LoadSleepPrice = loadFactor.MehrcomParsLoadFactor.LoadSleepPrice,
                LoadSleepTime = loadFactor.MehrcomParsLoadFactor.LoadSleepTime
            };
        }

        public async Task<bool> SequenceExistInMehrcomPars(long id, long sequence)
        {
            return await db.MehrcomParsLoadFactor.AsNoTracking().AnyAsync(a => a.Sequence.Equals(sequence) && !a.Id.Equals(id));
        }

        public async Task<bool> CheckMehrcomParsLoadFactorGovNumber(string number)
        {
            return await db.MehrcomParsLoadFactor.AsNoTracking().AnyAsync(a => a.LoadNumberGovReturn.Equals(number));
        }

        public void UpdateMehrcomPars(MehrcomParsLoadFactor obj)
        {
            db.Update(obj);
        }

        public void CreateMehrcomPars(MehrcomParsLoadFactor obj)
        {
            db.Add(obj);
        }

        public void DeleteMehrcomPars(MehrcomParsLoadFactor obj)
        {
            db.Remove(obj);
        }

        public void UpdateSaipaPlasco(SaipaPlascoLoadFactor obj)
        {
            db.Update(obj);
        }

        public void CreateSaipaPlasco(SaipaPlascoLoadFactor obj)
        {
            db.Add(obj);
        }
    }
}

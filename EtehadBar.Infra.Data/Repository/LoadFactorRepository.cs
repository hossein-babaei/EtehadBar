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
                //Sequence = loadFactor.SaipaPlascoLoadFactor.Sequence,
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
                AccountBookId = loadFactor.AccountBookId,
                IsFreeDriverPrice = loadFactor.IsFreeDriverPrice,
            };
        }

        public async Task<ESaipaPressLoadFactorVM> GetSaipaPressLoadFactor(long loadFactorId)
        {
            var loadFactor = await db.LoadFactor.Include(a => a.SaipaPressLoadFactor).SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
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
                AccountBookId = loadFactor.AccountBookId,
                IsFreeDriverPrice = loadFactor.IsFreeDriverPrice
            };
        }

        public async Task<ESazehGostarLoadFactorVM> GetSazehGostarLoadFactor(long loadFactorId)
        {
            var loadFactor = await db.LoadFactor.Include(a => a.SazehGostarLoadFactor).SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
            if (loadFactor == null) return new ESazehGostarLoadFactorVM();

            var pd = new PersianDateTime(loadFactor.Date);
            return new ESazehGostarLoadFactorVM
            {
                //Sequence = loadFactor.SazehGostarLoadFactor.Sequence,
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
                AccountBookId = loadFactor.AccountBookId,
                SazehLoadType = loadFactor.SazehGostarLoadFactor.SazehLoadType,
                IsFreeDriverPrice = loadFactor.IsFreeDriverPrice
            };
        }

        public async Task<List<ExcelLoadFactorVM>> LoadFactors(long customerId, long? calendarId, long? accountBookId, long? driverId)
        {
            if (!calendarId.HasValue)
            {
                var data = from a in db.LoadFactor.Include(a => a.MehrcomParsLoadFactor).ThenInclude(a => a.Category)
                           join b in db.Contract on a.ContractId equals b.Id
                           join c in db.AccountBook on a.AccountBookId equals c.Id
                           join d in db.ShippingFee on a.ShippingFeeId equals d.Id
                           join e in db.Vehicles on a.VehicleId equals e.Id
                           join f in db.Driver on a.DriverId equals f.Id
                           join calendar in db.Calendar on a.CalendarId equals calendar.Id
                           join g in db.LoadRoute on a.OriginId equals g.Id
                           where g.RouteType == LoadRouteType.Origin
                           join h in db.LoadRoute on a.DestinationId equals h.Id
                           where h.RouteType == LoadRouteType.Destionation
                           where b.CustomerId.Equals(customerId)
                           select new ExcelLoadFactorVM
                           {
                               Id = a.Id,
                               AccountBookId = a.AccountBookId,
                               DriverId = a.DriverId,
                               AccountBookNumber = c.Number,
                               AdminId = a.AdminId,
                               Amount = a.Amount,
                               CalendarEndDate = calendar.EndDate,
                               CalendarId = calendar.Id,
                               CalendarStartDate = calendar.StartDate,
                               CalendarTitle = calendar.Title,
                               ContractId = a.ContractId,
                               CreateDateTime = a.CreateDateTime,
                               Date = a.Date,
                               DestinationName = h.Title,
                               OriginName = g.Title,
                               DriverFee = a.DriverFee,
                               DriverLoadSleepPrice = a.DriverLoadSleepPrice,
                               DriverName = f.Fullname,
                               DriverTonnagePrice = a.DriverTonnagePrice,
                               ExitNumber = a.ExitNumber,
                               IsDriverFeeEditedByAdmin = a.IsDriverFeeEditedByAdmin,
                               IsFreeDriverPrice = a.IsFreeDriverPrice,
                               LoadFactorDeductions = a.LoadFactorDeductions,
                               LoadNumber = a.LoadNumber,
                               LoadNumberGov = a.LoadNumberGov,
                               LoadSleepPrice = a.LoadSleepPrice,
                               LoadSleepTime = a.LoadSleepTime,
                               MehrcomParsLoadFactor = a.MehrcomParsLoadFactor,
                               SaipaPlascoLoadFactor = a.SaipaPlascoLoadFactor,
                               SaipaPressLoadFactor = a.SaipaPressLoadFactor,
                               SazehGostarLoadFactor = a.SazehGostarLoadFactor,
                               Tonnage = a.Tonnage,
                               TonnagePrice = a.TonnagePrice,
                               VAT = a.VAT,
                               VehicleId = a.VehicleId,
                               VehicleIranStateNumber = e.IranStateNumber,
                               VehicleLeftNumber = e.LeftNumber,
                               VehicleName = d.Vehicle,
                               VehicleNumberWord = e.NumberWord,
                               VehicleRightNumber = e.RightNumber,
                               WeighbridgePrice = a.WeighbridgePrice,
                               WithholdingTax = a.WithholdingTax
                           };

                return await data.Where(a => accountBookId.HasValue ? a.AccountBookId.Equals(accountBookId.Value) : true &&
                driverId.HasValue ? a.DriverId.Equals(driverId.Value) : true).OrderBy(a => a.Date).ToListAsync();
            }
            else
            {
                var data = from a in db.LoadFactor
                           join b in db.Contract on a.ContractId equals b.Id
                           join c in db.AccountBook on a.AccountBookId equals c.Id
                           join d in db.ShippingFee on a.ShippingFeeId equals d.Id
                           join e in db.Vehicles on a.VehicleId equals e.Id
                           join f in db.Driver on a.DriverId equals f.Id
                           join calendar in db.Calendar on a.CalendarId equals calendar.Id
                           join g in db.LoadRoute on a.OriginId equals g.Id
                           where g.RouteType == LoadRouteType.Origin
                           join h in db.LoadRoute on a.DestinationId equals h.Id
                           where h.RouteType == LoadRouteType.Destionation
                           where a.CalendarId.Equals(calendarId.Value) && b.CustomerId.Equals(customerId)
                           select new ExcelLoadFactorVM
                           {
                               Id = a.Id,
                               AccountBookId = a.AccountBookId,
                               DriverId = a.DriverId,
                               AccountBookNumber = c.Number,
                               AdminId = a.AdminId,
                               Amount = a.Amount,
                               CalendarEndDate = calendar.EndDate,
                               CalendarId = calendar.Id,
                               CalendarStartDate = calendar.StartDate,
                               CalendarTitle = calendar.Title,
                               ContractId = a.ContractId,
                               CreateDateTime = a.CreateDateTime,
                               Date = a.Date,
                               DestinationName = h.Title,
                               OriginName = g.Title,
                               DriverFee = a.DriverFee,
                               DriverLoadSleepPrice = a.DriverLoadSleepPrice,
                               DriverName = f.Fullname,
                               DriverTonnagePrice = a.DriverTonnagePrice,
                               ExitNumber = a.ExitNumber,
                               IsDriverFeeEditedByAdmin = a.IsDriverFeeEditedByAdmin,
                               IsFreeDriverPrice = a.IsFreeDriverPrice,
                               LoadFactorDeductions = a.LoadFactorDeductions,
                               LoadNumber = a.LoadNumber,
                               LoadNumberGov = a.LoadNumberGov,
                               LoadSleepPrice = a.LoadSleepPrice,
                               LoadSleepTime = a.LoadSleepTime,
                               MehrcomParsLoadFactor = a.MehrcomParsLoadFactor,
                               SaipaPlascoLoadFactor = a.SaipaPlascoLoadFactor,
                               SaipaPressLoadFactor = a.SaipaPressLoadFactor,
                               SazehGostarLoadFactor = a.SazehGostarLoadFactor,
                               Tonnage = a.Tonnage,
                               TonnagePrice = a.TonnagePrice,
                               VAT = a.VAT,
                               VehicleId = a.VehicleId,
                               VehicleIranStateNumber = e.IranStateNumber,
                               VehicleLeftNumber = e.LeftNumber,
                               VehicleName = d.Vehicle,
                               VehicleNumberWord = e.NumberWord,
                               VehicleRightNumber = e.RightNumber,
                               WeighbridgePrice = a.WeighbridgePrice,
                               WithholdingTax = a.WithholdingTax
                           };

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
            var loadFactor = await db.LoadFactor.Include(a => a.MehrcomParsLoadFactor).SingleOrDefaultAsync(a => a.Id.Equals(loadFactorId));
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
                LoadNumberGov = loadFactor.LoadNumberGov,
                LoadNumberGovReturn = loadFactor.MehrcomParsLoadFactor.LoadNumberGovReturn,
                Palette = loadFactor.MehrcomParsLoadFactor.Palette,
                Return = loadFactor.MehrcomParsLoadFactor.Return,
                WeighbridgePrice = loadFactor.WeighbridgePrice,
                CategoryId = loadFactor.MehrcomParsLoadFactor.CategoryId,
                DriverLoadSleepPrice = loadFactor.DriverLoadSleepPrice,
                DriverTonnagePrice = loadFactor.DriverTonnagePrice,
                TonnagePrice = loadFactor.TonnagePrice,
                LoadSleepPrice = loadFactor.LoadSleepPrice,
                LoadSleepTime = loadFactor.LoadSleepTime,
                HasAddonMessage = loadFactor.MehrcomParsLoadFactor.HasAddonMessage,
                IsFreeDriverPrice = loadFactor.IsFreeDriverPrice
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

using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using MD.PersianDateTime.Standard;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class LoadFactorNovinRepository : ILoadFactorNovinRepository
    {
        private readonly ApplicationDbContext db;

        public LoadFactorNovinRepository(ApplicationDbContext context)
        {
            db = context;
        }
        public void Create(LoadFactorNovin obj)
        {
            db.Add(obj);
        }

        public void Delete(LoadFactorNovin obj)
        {
            db.Remove(obj);
        }

        public async Task<LoadFactorNovin> Get(long id)
        {
            return await db.LoadFactorNovin.FindAsync(id);
        }

        public async Task<LoadFactorNovin> Get(string rowId)
        {
            return await db.LoadFactorNovin.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<EditLoadFactorNovinVM> GetEditData(long id)
        {
            var item = await db.LoadFactorNovin.FirstOrDefaultAsync(a => a.Id.Equals(id));
            var pd = new PersianDateTime(item.Date);
            var pPd = new PersianDateTime(item.PaymentDate);
            var rPd = new PersianDateTime(item.ReceiveDate);
            return new EditLoadFactorNovinVM
            {
                Amount = item.Amount,
                Code = item.Code,
                Id = item.Id,
                ApplicantName = item.ApplicantName,
                CalendarId = item.CalendarId,
                CustomerId = item.CustomerId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                Destination = item.Destination,
                DriverFee = item.DriverFee,
                DriverId = item.DriverId,
                DriverTonnagePrice = item.DriverTonnagePrice,
                LoadNumber = item.LoadNumber,
                LoadNumberGov = item.LoadNumberGov,
                Origin = item.Origin,
                Tonnage = item.Tonnage,
                TonnagePrice = item.TonnagePrice,
                VehicleId = item.VehicleId,
                IsPaied = item.IsPaied,
                IsReceived = item.IsReceived,
                PDay = pPd.Year == 1 ? 0 : pPd.Day,
                PMonth = pPd.Year == 1 ? 0 : pPd.Month,
                PYear = pPd.Year == 1 ? 0 : pPd.Year,
                RDay = rPd.Year == 1 ? 0 : rPd.Day,
                RMonth = rPd.Year == 1 ? 0 : rPd.Month,
                RYear = rPd.Year == 1 ? 0 : rPd.Year
            };
        }

        public IQueryable<LoadFactorNovin> Query()
        {
            return db.LoadFactorNovin.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(LoadFactorNovin obj)
        {
            db.Update(obj);
        }
    }
}

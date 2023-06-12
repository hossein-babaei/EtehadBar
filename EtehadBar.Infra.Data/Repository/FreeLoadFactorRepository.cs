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
    public class FreeLoadFactorRepository : IFreeLoadFactorRepository
    {
        private readonly ApplicationDbContext db;

        public FreeLoadFactorRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(FreeLoadFactor obj)
        {
            db.Add(obj);
        }

        public void Delete(FreeLoadFactor obj)
        {
            db.Remove(obj);
        }

        public async Task<FreeLoadFactor> Get(long id)
        {
            return await db.FreeLoadFactor.FindAsync(id);
        }

        public async Task<EditFreeLoadFactorVM> GetEditData(long id)
        {
            var item = await db.FreeLoadFactor.AsNoTracking().FirstOrDefaultAsync(a => a.Id.Equals(id));
            var pd = new PersianDateTime(item.Date);
            return new EditFreeLoadFactorVM
            {
                Id = item.Id,
                Amount = item.Amount,
                ApplicantName = item.ApplicantName,
                CalendarId = item.CalendarId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
                Destination = item.Destination,
                DriverFee = item.DriverFee,
                DriverName = item.DriverName,
                DriverTonnagePrice = item.DriverTonnagePrice,
                LoadNumber = item.LoadNumber,
                LoadNumberGov = item.LoadNumberGov,
                Origin = item.Origin,
                Tonnage = item.Tonnage,
                TonnagePrice = item.TonnagePrice,
                VehicleType = item.VehicleType,
                DriverNationalNumber = item.DriverNationalNumber,
                IranStateNumber = item.IranStateNumber,
                LeftNumber = item.LeftNumber,
                RightNumber = item.RightNumber,
                NumberWord = item.NumberWord,
                LoadFactorScan = item.LoadFactorScan
            };
        }

        public IQueryable<FreeLoadFactor> Query()
        {
            return db.FreeLoadFactor.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(FreeLoadFactor obj)
        {
            db.Update(obj);
        }
    }
}

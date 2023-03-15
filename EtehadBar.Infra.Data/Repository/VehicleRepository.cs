using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext db;

        public VehicleRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Vehicle obj)
        {
            db.Add(obj);
        }

        public IQueryable<Vehicle> Vehicles()
        {
            return db.Vehicles.AsQueryable();
        }

        public async Task<Vehicle> Get(long id)
        {
            return await db.Vehicles.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Vehicle obj)
        {
            db.Update(obj);
        }

        public async Task<List<ActivityListVM>> ActivityList(long customerId, long calendarId)
        {
            var query = await (from a in db.LoadFactor
                               join b in db.Contract on a.ContractId equals b.Id
                               join c in db.Vehicles on a.VehicleId equals c.Id
                               where a.CalendarId.Equals(calendarId) && b.CustomerId.Equals(customerId)
                               select new
                               {
                                   a.Tonnage,
                                   a.DriverTonnagePrice,
                                   a.DriverFee,
                                   c.AccountBankName,
                                   c.BankAccountNumber,
                                   VehicleNumber = $"ایران {c.IranStateNumber} - {c.RightNumber} {c.NumberWord} {c.LeftNumber}",
                                   a.WeighbridgePrice,
                                   a.DriverLoadSleepPrice,
                                   a.VehicleId,
                                   c.VehicleOwnerFullname
                               }).AsNoTracking().ToListAsync();

            var data = new List<ActivityListVM>();
            foreach (var vehicle in query.DistinctBy(a => a.VehicleId))
            {

                var thisVehicleActivity = query.Where(a => a.VehicleId.Equals(vehicle.VehicleId)).ToList();
                var driverFee = 0d;
                foreach (var item in thisVehicleActivity)
                {
                    driverFee = item.DriverFee;
                    if (item.Tonnage.HasValue)
                        driverFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                    if (item.WeighbridgePrice.HasValue)
                        driverFee += item.WeighbridgePrice.Value;

                    if (item.DriverLoadSleepPrice.HasValue)
                        driverFee += item.DriverLoadSleepPrice.Value;
                }

                data.Add(new ActivityListVM
                {
                    VehicleId = vehicle.VehicleId,
                    BankAccountNumber = vehicle.BankAccountNumber,
                    VehicleNumber = vehicle.VehicleNumber,
                    VehicleOwnerName = vehicle.VehicleOwnerFullname,
                    Amount = driverFee
                });
            }
            return data;
        }
    }
}

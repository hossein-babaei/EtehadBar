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

        public async Task<List<ActivityListVM>> ActivityList(long customerId, long calendarId, bool hasPayment)
        {
            var query = await (from a in db.LoadFactor
                               join b in db.Contract on a.ContractId equals b.Id
                               join c in db.Vehicles on a.VehicleId equals c.Id
                               where a.CalendarId.Equals(calendarId) && b.CustomerId.Equals(customerId) && !a.IsFreeDriverPrice
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

            var payments = new List<ActivityListPaymentVM>();
            if (hasPayment)
            {
                var vehicleIdList = query.Select(a => a.VehicleId).Distinct().ToList();
                payments = await db.Payment.Where(a => a.CalendarId.Equals(calendarId) && a.VehicleId.HasValue && vehicleIdList.Contains(a.VehicleId.Value))
                    .Select(a => new ActivityListPaymentVM { VehicleId = a.VehicleId.Value, Amount = a.Amount }).AsNoTracking().ToListAsync();
            }

            var data = new List<ActivityListVM>();
            foreach (var vehicle in query.DistinctBy(a => a.VehicleId))
            {

                var thisVehicleActivity = query.Where(a => a.VehicleId.Equals(vehicle.VehicleId)).ToList();
                var driverFee = 0d;
                foreach (var item in thisVehicleActivity)
                {
                    driverFee += item.DriverFee;
                    if (item.Tonnage.HasValue)
                        driverFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                    if (item.WeighbridgePrice.HasValue)
                        driverFee += item.WeighbridgePrice.Value;

                    if (item.DriverLoadSleepPrice.HasValue)
                        driverFee += item.DriverLoadSleepPrice.Value;
                }

                if (hasPayment)
                {
                    var thisVehiclePaymnets = payments.Where(a => a.VehicleId.Equals(vehicle.VehicleId)).Sum(a => a.Amount);
                    driverFee -= thisVehiclePaymnets;
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

        public async Task<List<ActivityListByCustomerVM>> ActivityListByCustomer(long customerId, long calendarId)
        {
            var query = await (from a in db.LoadFactor
                               join b in db.Contract on a.ContractId equals b.Id
                               join c in db.Vehicles on a.VehicleId equals c.Id
                               where a.CalendarId.Equals(calendarId) && b.CustomerId.Equals(customerId) && !a.IsFreeDriverPrice
                               select new
                               {
                                   VehicleId = c.Id,
                                   c.LeftNumber,
                                   a.Date,
                                   DriverName = a.Driver.Fullname,
                                   Origin = a.Origin.Title,
                                   Destination = a.Destination.Title,
                                   a.IsFreeDriverPrice,
                                   a.Tonnage,
                                   a.DriverTonnagePrice,
                                   a.DriverFee,
                                   VehicleNumber = $"ایران {c.IranStateNumber} - {c.RightNumber} {c.NumberWord} {c.LeftNumber}",
                                   a.WeighbridgePrice,
                                   a.DriverLoadSleepPrice,
                                   a.LoadNumber,
                                   SazehRequestNumber = a.ExitNumber,
                                   PressFloorType = a.SaipaPressLoadFactor != null ? a.SaipaPressLoadFactor.PressFloorType : SaipaPressLoadType.OneFloor,
                                   MehrcomLoad = a.MehrcomParsLoadFactor != null && a.MehrcomParsLoadFactor.Load,
                                   MehrcomPalette = a.MehrcomParsLoadFactor != null && a.MehrcomParsLoadFactor.Palette,
                                   MehrcomReturn = a.MehrcomParsLoadFactor != null && a.MehrcomParsLoadFactor.Return,

                               }).AsNoTracking().OrderBy(a => a.LeftNumber).ToListAsync();

            var data = new List<ActivityListByCustomerVM>();
            var vehicleData = query.GroupBy(a => a.VehicleNumber).ToList();

            var calendars = await db.Calendar.AsNoTracking().Where(a => a.Sequence >= (db.Calendar.AsNoTracking().Max(a => a.Sequence) - 6)).Select(a => a.Id).ToListAsync();

            foreach (var vehicle in vehicleData)
            {
                var thisVehicle = new ActivityListByCustomerVM
                {
                    VehicleNumber = vehicle.Key,
                    Routes = new List<ActivityListByCustomerRouteVM>(),
                    Details = new List<ActivityListByCustomerDetailVM>()
                };
                var priceGroups = vehicle.DistinctBy(a => a.DriverFee).Select(a => a.DriverFee).ToList();
                foreach (var price in priceGroups)
                {
                    thisVehicle.Routes.Add(new ActivityListByCustomerRouteVM
                    {
                        Amount = price,
                        Quantity = vehicle.Count(a => a.DriverFee.Equals(price))
                    });
                }
                for (int i = 0; i < vehicle.Count(); i++)
                {
                    var item = vehicle.ElementAt(i);
                    if (thisVehicle.VehicleId == 0) { thisVehicle.VehicleId = item.VehicleId; };
                    thisVehicle.Details.Add(new ActivityListByCustomerDetailVM
                    {
                        Amount = item.DriverFee,
                        Date = item.Date,
                        Destination = item.Destination,
                        DriverLoadSleepPrice = item.DriverLoadSleepPrice,
                        DriverName = item.DriverName,
                        IsFreeDriverPrice = item.IsFreeDriverPrice,
                        LoadFactorNumber = item.LoadNumber,
                        MehrcomLoad = item.MehrcomLoad,
                        MehrcomPalette = item.MehrcomPalette,
                        MehrcomReturn = item.MehrcomReturn,
                        Origin = item.Origin,
                        PressFloorType = item.PressFloorType,
                        SazehRequestNumber = item.SazehRequestNumber,
                        WeighbridgePrice = item.WeighbridgePrice,
                        Tonnage = item.Tonnage,
                        TonnagePrice = item.DriverTonnagePrice
                    });
                }
                data.Add(thisVehicle);
            }

            return data;
        }
    }
}

using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<List<ActivityListVM>> ActivityList(long customerId, long calendarId, bool hasPayment, bool isFreeDriverPrice = false)
        {
            var query = await (from a in db.LoadFactor
                               join b in db.Contract on a.ContractId equals b.Id
                               join c in db.Vehicles on a.VehicleId equals c.Id
                               where a.CalendarId.Equals(calendarId) && b.CustomerId.Equals(customerId) && a.IsFreeDriverPrice.Equals(isFreeDriverPrice)
                               select new
                               {
                                   a.Tonnage,
                                   a.DriverTonnagePrice,
                                   a.DriverFee,
                                   c.LeftNumber,
                                   VehicleNumber = $"ایران {c.IranStateNumber} - {c.RightNumber} {c.NumberWord} {c.LeftNumber}",
                                   a.WeighbridgePrice,
                                   a.DriverLoadSleepPrice,
                                   a.VehicleId,
                                   c.VehicleOwnerFullname
                               }).AsNoTracking().OrderBy(a => a.LeftNumber).ToListAsync();

            var vehicleBalances = new List<ActivityListPaymentVM>();

            var vehicleIdList = query.Select(a => a.VehicleId).Distinct().ToList();

            var vehicleBankAccountList = await db.VehicleBankAccount.Include(a => a.Definition).Where(a => vehicleIdList.Contains(a.VehicleId)).ToListAsync();
            var customerBank = await db.Customer.AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.ActiveBank).FirstAsync();

            if (hasPayment)
            {
                var thisCalendarSequence = await db.Calendar.AsNoTracking().Where(a => a.Id.Equals(calendarId)).Select(a => a.Sequence).SingleAsync();
                var calendars = await db.Calendar.AsNoTracking().Where(a => a.Sequence <= thisCalendarSequence).Select(a => a.Id).ToListAsync();
                vehicleBalances = await db.VehicleBalance
                    .Where(a => (a.CustomerId.HasValue ? a.CustomerId.Value.Equals(customerId) : true) &&
                    a.CalendarId.HasValue && calendars.Contains(a.CalendarId.Value) && vehicleIdList.Contains(a.VehicleId))
                    .Select(a => new ActivityListPaymentVM { VehicleId = a.VehicleId, Amount = a.Amount }).AsNoTracking().ToListAsync();
            }

            var data = new List<ActivityListVM>();
            foreach (var vehicle in query.DistinctBy(a => a.VehicleId))
            {
                var thisVehicleBankItem = vehicleBankAccountList.FirstOrDefault(a => a.VehicleId.Equals(vehicle.VehicleId) && a.BankId.Equals(customerBank));
                thisVehicleBankItem ??= vehicleBankAccountList.FirstOrDefault(a => a.VehicleId.Equals(vehicle.VehicleId));

                if (hasPayment)
                {
                    var thisVehicleBalance = vehicleBalances.Where(a => a.VehicleId.Equals(vehicle.VehicleId)).Sum(a => a.Amount);
                    data.Add(new ActivityListVM
                    {
                        VehicleId = vehicle.VehicleId,
                        BankAccountNumber = thisVehicleBankItem == null ? "---" : string.IsNullOrWhiteSpace(thisVehicleBankItem.AccountNumber) ? "---" : thisVehicleBankItem.BankId.Equals(customerBank) ? thisVehicleBankItem.AccountNumber : $"{thisVehicleBankItem.AccountNumber} ({thisVehicleBankItem.Definition.Title})",
                        VehicleNumber = vehicle.VehicleNumber,
                        VehicleOwnerName = thisVehicleBankItem == null ? vehicle.VehicleOwnerFullname : thisVehicleBankItem.Fullname,
                        Amount = thisVehicleBalance < 0 ? 0 : thisVehicleBalance,
                    });
                }
                else
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

                    data.Add(new ActivityListVM
                    {
                        VehicleId = vehicle.VehicleId,
                        BankAccountNumber = thisVehicleBankItem == null ? "---" : string.IsNullOrWhiteSpace(thisVehicleBankItem.AccountNumber) ? "---" : !thisVehicleBankItem.BankId.Equals(customerBank) ? thisVehicleBankItem.AccountNumber : $"{thisVehicleBankItem.AccountNumber} ({thisVehicleBankItem.Definition.Title})",
                        VehicleNumber = vehicle.VehicleNumber,
                        VehicleOwnerName = thisVehicleBankItem == null ? vehicle.VehicleOwnerFullname : thisVehicleBankItem.Fullname,
                        Amount = driverFee
                    });
                }
            }
            return data;
        }
         
        public async Task<List<ActivityListByCustomerVM>> ActivityListByCustomer(long customerId, long calendarId, bool isFreeDriverPrice = false)
        {
            var query = await (from a in db.LoadFactor
                               join b in db.Contract on a.ContractId equals b.Id
                               join c in db.Vehicles on a.VehicleId equals c.Id
                               where a.CalendarId.Equals(calendarId) && b.CustomerId.Equals(customerId) && a.IsFreeDriverPrice.Equals(isFreeDriverPrice)
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
                                   a.LoadNumberGov,
                                   VehicleType = c.Type,
                                   SazehRequestNumber = a.ExitNumber,
                                   PressFloorType = a.SaipaPressLoadFactor != null ? a.SaipaPressLoadFactor.PressFloorType : SaipaPressLoadType.OneFloor,
                                   MehrcomLoad = a.MehrcomParsLoadFactor != null && a.MehrcomParsLoadFactor.Load,
                                   MehrcomPalette = a.MehrcomParsLoadFactor != null && a.MehrcomParsLoadFactor.Palette,
                                   MehrcomReturn = a.MehrcomParsLoadFactor != null && a.MehrcomParsLoadFactor.Return

                               }).AsNoTracking().OrderBy(a => a.LeftNumber).ToListAsync();

            var customerInfo = await db.Customer.FindAsync(customerId);

            var data = new List<ActivityListByCustomerVM>();
            var vehicleData = query.GroupBy(a => a.VehicleNumber).ToList();

            var vehicleIds = query.Select(a => a.VehicleId).Distinct().ToList();
            var thisCalendarSequence = await db.Calendar.AsNoTracking().Where(a => a.Id.Equals(calendarId)).Select(a => a.Sequence).SingleAsync();
            var calendars = await db.Calendar.AsNoTracking().Where(a => a.Sequence <= thisCalendarSequence).Select(a => a.Id).ToListAsync();
            var vehicleBalance = await db.VehicleBalance.AsNoTracking().Where(a =>
            (a.CustomerId.HasValue ? a.CustomerId.Value.Equals(customerId) : true) &&
            a.CalendarId.HasValue && calendars.Contains(a.CalendarId.Value) && vehicleIds.Contains(a.VehicleId))
                .Select(a => new
                {
                    a.VehicleId,
                    a.Amount
                }).ToListAsync();

            //var calendars = await db.Calendar.AsNoTracking().Where(a => a.Sequence <= thisCalendarSequence && a.Sequence >= (thisCalendarSequence - 6)).Select(a => a.Id).ToListAsync();
            //var payments = await db.Bill.Where(a => a.VehicleId.HasValue && vehicleIds.Contains(a.VehicleId.Value) && calendars.Contains(a.CalendarId)).AsNoTracking().Where(a => !a.BillType.Equals("موردی"))
            //    .Select(a => new
            //    {
            //        a.VehicleId,
            //        a.Amount,
            //        a.BillType
            //    }).ToListAsync();
            //var sixMonthActivity = await (from a in db.LoadFactor
            //                              join b in db.Contract on a.ContractId equals b.Id
            //                              where b.CustomerId.Equals(customerId) && vehicleIds.Contains(a.VehicleId) && calendars.Contains(a.CalendarId) && !a.IsFreeDriverPrice
            //                              select new
            //                              {
            //                                  a.VehicleId,
            //                                  a.DriverFee,
            //                                  a.Tonnage,
            //                                  a.DriverTonnagePrice,
            //                                  a.DriverLoadSleepPrice,
            //                                  a.LoadSleepTime,
            //                                  a.WeighbridgePrice
            //                              }).AsNoTracking().ToListAsync();

            var vehicleBankAccounts = await db.VehicleBankAccount.AsNoTracking().Where(a => vehicleIds.Contains(a.VehicleId)).Select(a => new VehicleBankAccountVM
            {
                VehicleId = a.VehicleId,
                AccountNumber = a.AccountNumber,
                BankId = a.BankId,
                Fullname = a.Fullname
            }).ToListAsync();

            foreach (var vehicle in vehicleData)
            {
                var vehicleId = vehicle.ElementAt(0).VehicleId;
                var thisVehicle = new ActivityListByCustomerVM
                {
                    VehicleType = vehicle.ElementAt(0).VehicleType,
                    VehicleNumber = vehicle.Key,
                    VehicleBalance = vehicleBalance.Where(a => a.VehicleId.Equals(vehicleId)).Sum(a => a.Amount),
                    BankAccounts = vehicleBankAccounts.Where(a => a.VehicleId.Equals(vehicleId)).ToList(),
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
                    
                    thisVehicle.Details.Add(new ActivityListByCustomerDetailVM
                    {
                        Amount = item.DriverFee,
                        Date = item.Date,
                        Destination = item.Destination,
                        DriverLoadSleepPrice = item.DriverLoadSleepPrice,
                        DriverName = item.DriverName,
                        IsFreeDriverPrice = item.IsFreeDriverPrice,
                        LoadFactorNumber = customerInfo.CustomerType == CustomerType.SaipaPlasco ? (string.IsNullOrWhiteSpace(item.LoadNumberGov) ? item.LoadNumber : item.LoadNumberGov) : item.LoadNumber,
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

        public async Task<List<ActivityListVM>> FullActivityList(long customerId, long calendarId, int type)
        {
            bool binaryType = Convert.ToBoolean(type);

            var query = await(from a in db.LoadFactor
                              join b in db.Contract on a.ContractId equals b.Id
                              join c in db.Vehicles on a.VehicleId equals c.Id
                              where a.CalendarId.Equals(calendarId) && b.CustomerId.Equals(customerId)
                              where binaryType || !a.IsFreeDriverPrice
                              select new
                              {
                                  a.Tonnage,
                                  a.DriverTonnagePrice,
                                  a.DriverFee,
                                  c.LeftNumber,
                                  c.RightNumber,
                                  VehicleNumber = $"ایران {c.IranStateNumber} - {c.RightNumber} {c.NumberWord} {c.LeftNumber}",
                                  a.WeighbridgePrice,
                                  a.DriverLoadSleepPrice,
                                  a.VehicleId,
                                  c.VehicleOwnerFullname
                              }).AsNoTracking().OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).ToListAsync();

            var vehicleIdList = query.Select(a => a.VehicleId).Distinct().ToList();
            var vehicleBankAccountList = await db.VehicleBankAccount.Include(a => a.Definition).Where(a => vehicleIdList.Contains(a.VehicleId)).ToListAsync();
            var customerBank = await db.Customer.AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.ActiveBank).FirstAsync();
            var thisCalendarSequence = await db.Calendar.AsNoTracking().Where(a => a.Id.Equals(calendarId)).Select(a => a.Sequence).SingleAsync();
            var calendars = await db.Calendar.AsNoTracking().Where(a => a.Sequence <= thisCalendarSequence).Select(a => a.Id).ToListAsync();
            var vehicleBalances = await db.VehicleBalance
                    .Where(a => (a.CustomerId.HasValue ? a.CustomerId.Value.Equals(customerId) : true) && a.BillId.HasValue &&
                    a.CalendarId.HasValue && a.CalendarId.Value.Equals(calendarId) /*calendars.Contains(a.CalendarId.Value)*/ && vehicleIdList.Contains(a.VehicleId))
                    .Select(a => new ActivityListPaymentVM { VehicleId = a.VehicleId, Amount = a.Amount }).AsNoTracking().ToListAsync();

            var data = new List<ActivityListVM>();
            foreach (var vehicle in query.DistinctBy(a => a.VehicleId))
            {
                var thisVehicleBankItem = vehicleBankAccountList.FirstOrDefault(a => a.VehicleId.Equals(vehicle.VehicleId) && a.BankId.Equals(customerBank));
                thisVehicleBankItem ??= vehicleBankAccountList.FirstOrDefault(a => a.VehicleId.Equals(vehicle.VehicleId));

                var thisVehicleBalance = vehicleBalances.Where(a => a.VehicleId.Equals(vehicle.VehicleId)).Sum(a => a.Amount);

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

                data.Add(new ActivityListVM
                {
                    VehicleId = vehicle.VehicleId,
                    BankAccountNumber = thisVehicleBankItem == null ? "---" : string.IsNullOrWhiteSpace(thisVehicleBankItem.AccountNumber) ? "---" : thisVehicleBankItem.BankId.Equals(customerBank) ? thisVehicleBankItem.AccountNumber : $"{thisVehicleBankItem.AccountNumber} ({thisVehicleBankItem.Definition.Title})",
                    VehicleNumber = vehicle.VehicleNumber,
                    VehicleOwnerName = thisVehicleBankItem == null ? vehicle.VehicleOwnerFullname : thisVehicleBankItem.Fullname,
                    VehicleLeftNumber = vehicle.LeftNumber,
                    VehicleRightNumber = vehicle.RightNumber,
                    Amount = -thisVehicleBalance,
                    ActivityAmount = driverFee
                });
            }
            return data;
        }
    }
}

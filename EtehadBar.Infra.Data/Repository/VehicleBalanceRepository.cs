using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class VehicleBalanceRepository : IVehicleBalanceRepository
    {
        private readonly ApplicationDbContext db;

        public VehicleBalanceRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task Create(VehicleBalance obj)
        {
            await db.AddAsync(obj);
        }

        public void Delete(VehicleBalance obj)
        {
            db.Remove(obj);
        }

        public async Task<VehicleBalance> Get(long id)
        {
            return await db.VehicleBalance.FindAsync(id);
        }

        public async Task<VehicleBalance> Get(string rowId)
        {
            return await db.VehicleBalance.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<List<VehicleBalance>> GetVehicleBalance(long vehicleId, long? calendarId)
        {
            var query = db.VehicleBalance.Where(a => a.VehicleId.Equals(vehicleId));
            if (calendarId.HasValue)
            {
                var calendars = await db.Calendar.AsNoTracking().Where(a => a.Sequence <= db.Calendar.Single(b => b.Id.Equals(calendarId.Value)).Sequence).Select(a => a.Id).ToListAsync();
                query = query.Where(a => a.CalendarId.HasValue && calendars.Contains(a.CalendarId.Value));
            }

            return await query.OrderBy(a => a.Id).ToListAsync();
        }

        public async Task<double> GetVehicleBalanceSum(long vehicleId, long? calendarId, long? customerId)
        {
            var query = db.VehicleBalance.Where(a => a.VehicleId.Equals(vehicleId));

            if (customerId.HasValue)
                query = query.Where(a => (a.CustomerId.HasValue ? a.CustomerId.Value.Equals(customerId.Value) : true));

            if (calendarId.HasValue)
            {
                var calendars = await db.Calendar.AsNoTracking().Where(a => a.Sequence <= db.Calendar.Single(b => b.Id.Equals(calendarId.Value)).Sequence).Select(a => a.Id).ToListAsync();
                query = query.Where(a => a.CalendarId.HasValue && calendars.Contains(a.CalendarId.Value));
            }

            return await query.SumAsync(a => a.Amount);
        }

        public IQueryable<VehicleBalance> Query()
        {
            return db.VehicleBalance.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(VehicleBalance obj)
        {
            db.Update(obj);
        }
    }
}

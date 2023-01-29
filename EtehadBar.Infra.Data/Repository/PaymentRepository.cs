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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext db;

        public PaymentRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Payment obj)
        {
            db.Add(obj);
        }

        public IQueryable<Payment> Payments()
        {
            return db.Payment.AsQueryable();
        }

        public void Delete(Payment obj)
        {
            db.Remove(obj);
        }

        public async Task<Payment> Get(long id)
        {
            return await db.Payment.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Payment obj)
        {
            db.Update(obj);
        }

        public async Task<List<PaymentVM>> PaymentVMList(long calendarId, byte? type, long? vehicleId)
        {
            var data = from a in db.Payment
                       join b in db.ApplicationUser on a.AdminId equals b.Id
                       join c in db.Vehicles on a.VehicleId equals c.Id
                       where a.CalendarId.Equals(calendarId)
                       select new PaymentVM
                       {
                           AdminId = a.AdminId,
                           Id = a.Id,
                           AdminName = $"{b.Firstname} {b.Lastname}",
                           Amount = a.Amount,
                           Date = a.Date,
                           Description = a.Description,
                           VehicleId = c.Id,
                           Vehicle = $"ایران {c.IranStateNumber} - {c.RightNumber} {c.NumberWord} {c.LeftNumber}",
                           Picture = a.Picture,
                           PaymentType = a.PaymentType
                       };

            if (type.HasValue)
            {
                if (type.Value != 2)
                    data = data.Where(a => a.PaymentType.Equals(type.Value));
            }

            if (vehicleId.HasValue)
            {
                if (vehicleId.Value != 0)
                    data = data.Where(a => a.VehicleId.Equals(vehicleId.Value));
            }

            return await data.AsNoTracking().ToListAsync();
        }
    }
}

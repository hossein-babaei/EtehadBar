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
    public class BillRepository : IBillRepository
    {
        private readonly ApplicationDbContext db;

        public BillRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Bill obj)
        {
            db.Add(obj);
        }

        public void Delete(Bill obj)
        {
            db.Remove(obj);
        }

        public void DeleteBillDetail(BillDetail obj)
        {
            db.Remove(obj);
        }

        public async Task<Bill> Get(long id)
        {
            return await db.Bill.FindAsync(id);
        }

        public async Task<Bill> Get(string rowId)
        {
            return await db.Bill.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<EditBillVM> GetEditData(long id)
        {
            var a = await db.Bill.Include(a => a.BillDetail).FirstOrDefaultAsync(a => a.Id.Equals(id));
            var date = new PersianDateTime(a.Date);
            return new EditBillVM
            {
                Id = a.Id,
                Amount = a.Amount,
                BankBillNo = a.BankBillNo,
                BillNo = a.BillNo,
                BankBranch = a.BankBranch,
                BillType = a.BillType,
                CalendarId = a.CalendarId,
                CustomerId = a.CustomerId,
                Description = a.Description,
                IsReturned = a.IsReturned,
                ReceiverName = a.ReceiverName,
                VehicleId = a.VehicleId,
                Year = date.Year,
                Month = date.Month,
                Day = date.Day,
                RealReceiverName = a.BillDetail is not null ? a.BillDetail.ReceiverName : "",
                ReceiverBankAccount = a.BillDetail is not null ? a.BillDetail.ReceiverBankAccount : ""
            };
        }

        public async Task<Bill> GetIncludedDetail(long id)
        {
            return await db.Bill.Include(a => a.BillDetail).FirstOrDefaultAsync(a => a.Id.Equals(id));
        }

        public IQueryable<Bill> Query()
        {
            return db.Bill.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Bill obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<Bill> bills)
        {
            db.UpdateRange(bills);
        }
    }
}

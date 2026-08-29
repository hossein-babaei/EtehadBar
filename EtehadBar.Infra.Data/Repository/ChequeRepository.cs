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
    public class ChequeRepository : IChequeRepository
    {
        private readonly ApplicationDbContext db;

        public ChequeRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Cheque obj)
        {
            db.Add(obj);
        }

        public void Delete(Cheque obj)
        {
            db.Remove(obj);
        }

        public async Task<Cheque> Get(long id)
        {
            return await db.Cheque.FindAsync(id);
        }

        public async Task<Cheque> Get(string rowId)
        {
            return await db.Cheque.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<EditChequeVM> GetEditData(long id)
        {
            var data = await db.Cheque.AsNoTracking().FirstOrDefaultAsync(a => a.Id.Equals(id));
            return new EditChequeVM
            {
                Id = data.Id,
                Amount = data.Amount,
                BankOfOrigin = data.BankOfOrigin,
                CustomerId = data.CustomerId,
                Date = new PersianDateTime(data.Date).ToString("yyyy/MM/dd"),
                Description = data.Description,
                Issuer = data.Issuer,
                Number = data.Number,
                RecieveDate = new PersianDateTime(data.RecieveDate).ToString("yyyy/MM/dd"),
                SendToBankName = data.SendToBankName,
                Status = data.Status,
                PassDate = data.PassDate.HasValue ? new PersianDateTime(data.PassDate.Value).ToString("yyyy/MM/dd") : null,
                SendToBankDate = data.SendToBankDate.HasValue ? new PersianDateTime(data.SendToBankDate.Value).ToString("yyyy/MM/dd") : null,
            };
        }

        public IQueryable<Cheque> Query()
        {
            return db.Cheque.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Cheque obj)
        {
            db.Update(obj);
        }
    }
}

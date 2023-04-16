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
    public class BankAccountBookRepository : IBankAccountBookRepository
    {
        private readonly ApplicationDbContext db;

        public BankAccountBookRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(BankAccountBook obj)
        {
            db.Add(obj);
        }

        public void Delete(BankAccountBook obj)
        {
            db.Remove(obj);
        }

        public async Task<BankAccountBook> Get(long id)
        {
            return await db.BankAccountBook.FindAsync(id);
        }

        public async Task<BankAccountBook> Get(string rowId)
        {
            return await db.BankAccountBook.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<EditBankAccountBookVM> GetEdit(long id)
        {
            var query = await db.BankAccountBook.Where(a => a.Id.Equals(id)).AsNoTracking().FirstOrDefaultAsync();
            var pd = new PersianDateTime(query.Date);
            var data = new EditBankAccountBookVM
            {
                Id = query.Id,
                Amount = query.Debtor > 0 ? query.Debtor : query.Creditor,
                AmountType = query.Debtor > 0 ? BankAccountBookAmountType.Debtor : BankAccountBookAmountType.Creditor,
                AccountBookType = query.AccountBookType,
                Description = query.Description,
                ReferenceNo = query.ReferenceNo,
                TransferFee = query.TransferFee,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year
            };
            return data;
        }

        public IQueryable<BankAccountBook> Query()
        {
            return db.BankAccountBook.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(BankAccountBook obj)
        {
            db.Update(obj);
        }
    }
}

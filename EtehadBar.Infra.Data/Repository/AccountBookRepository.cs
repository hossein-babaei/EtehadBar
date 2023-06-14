using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class AccountBookRepository : IAccountBookRepository
    {
        private readonly ApplicationDbContext db;

        public AccountBookRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<AccountBook> AccountBooks()
        {
            return db.AccountBook.AsQueryable();
        }

        public void Create(AccountBook obj)
        {
            db.Add(obj);
        }

        public void Delete(AccountBook obj)
        {
            db.Remove(obj);
        }

        public async Task<AccountBook> Get(long id)
        {
            return await db.AccountBook.FindAsync(id);
        }

        public async Task<AccountBook> Get(string rowId)
        {
            return await db.AccountBook.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(AccountBook obj)
        {
            db.Update(obj);
        }

        public async Task UpdateAmount(long accountBookId) 
        { 
            var accountBook = await db.AccountBook.FindAsync(accountBookId);
            var loadFactorAmount = await db.LoadFactor.AsNoTracking().Where(a => a.AccountBookId.Equals(accountBookId)).SumAsync(a =>
            a.Amount +
                    ((a.Tonnage.HasValue && a.TonnagePrice.HasValue) ? a.Tonnage.Value * a.TonnagePrice.Value : 0) +
                    (a.WeighbridgePrice.HasValue ? a.WeighbridgePrice.Value : 0) +
                    (a.DriverLoadSleepPrice.HasValue ? a.LoadSleepPrice.Value : 0));
            accountBook.Amount = loadFactorAmount;
            db.Update(accountBook);
        }
    }
}

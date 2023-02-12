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
    }
}

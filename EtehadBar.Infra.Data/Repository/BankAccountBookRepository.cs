using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
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

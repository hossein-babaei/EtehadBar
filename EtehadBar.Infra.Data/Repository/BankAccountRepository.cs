using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly ApplicationDbContext db;

        public BankAccountRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(BankAccount obj)
        {
            db.Add(obj);
        }

        public void Delete(BankAccount obj)
        {
            db.Remove(obj);
        }

        public async Task<BankAccount> Get(long id)
        {
            return await db.BankAccount.FindAsync(id);
        }

        public async Task<BankAccount> Get(string rowId)
        {
            return await db.BankAccount.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public IQueryable<BankAccount> Query()
        {
            return db.BankAccount.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(BankAccount obj)
        {
            db.Update(obj);
        }
    }
}

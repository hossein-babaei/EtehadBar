using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
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

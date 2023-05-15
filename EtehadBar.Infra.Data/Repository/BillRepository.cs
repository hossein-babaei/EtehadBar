using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Bill> Get(long id)
        {
            return await db.Bill.FindAsync(id);
        }

        public async Task<Bill> Get(string rowId)
        {
            return await db.Bill.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
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
    }
}

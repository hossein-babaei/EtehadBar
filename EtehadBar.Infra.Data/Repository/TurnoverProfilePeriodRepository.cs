using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class TurnoverProfilePeriodRepository : ITurnoverProfilePeriodRepository
    {
        private readonly ApplicationDbContext db;

        public TurnoverProfilePeriodRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(TurnoverProfilePeriod obj)
        {
            db.Add(obj);
        }

        public void Delete(TurnoverProfilePeriod obj)
        {
            db.Remove(obj);
        }

        public async Task<TurnoverProfilePeriod> Get(long id)
        {
            return await db.TurnoverProfilePeriod.FindAsync(id);
        }

        public async Task<TurnoverProfilePeriod> Get(string rowId)
        {
            return await db.TurnoverProfilePeriod.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public IQueryable<TurnoverProfilePeriod> Query()
        {
            return db.TurnoverProfilePeriod.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(TurnoverProfilePeriod obj)
        {
            db.Update(obj);
        }
    }
}

using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class TurnoverProfileRepository : ITurnoverProfileRepository
    {
        private readonly ApplicationDbContext db;

        public TurnoverProfileRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(TurnoverProfile obj)
        {
            db.Add(obj);
        }

        public void Delete(TurnoverProfile obj)
        {
            db.Remove(obj);
        }

        public async Task<TurnoverProfile> Get(long id)
        {
            return await db.TurnoverProfile.FindAsync(id);
        }

        public async Task<TurnoverProfile> Get(string rowId)
        {
            return await db.TurnoverProfile.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public IQueryable<TurnoverProfile> Query()
        {
            return db.TurnoverProfile.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(TurnoverProfile obj)
        {
            db.Update(obj);
        }
    }
}

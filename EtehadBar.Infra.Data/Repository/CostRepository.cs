using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class CostRepository : ICostRepository
    {
        private readonly ApplicationDbContext db;

        public CostRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Cost obj)
        {
            db.Add(obj);
        }

        public IQueryable<Cost> Costs()
        {
            return db.Cost.AsQueryable();
        }

        public void Delete(Cost obj)
        {
            db.Remove(obj);
        }

        public async Task<Cost> Get(long id)
        {
            return await db.Cost.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Cost obj)
        {
            db.Update(obj);
        }
    }
}

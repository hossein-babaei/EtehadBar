using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class TurnoverRepository : ITurnoverRepository
    {
        private readonly ApplicationDbContext db;

        public TurnoverRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Turnover obj)
        {
            db.Add(obj);
        }

        public void Delete(Turnover obj)
        {
            db.Remove(obj);
        }

        public async Task<Turnover> Get(long id)
        {
            return await db.Turnover.FindAsync(id);
        }

        public IQueryable<Turnover> Query()
        {
            return db.Turnover.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Turnover obj)
        {
            db.Update(obj);
        }
    }
}

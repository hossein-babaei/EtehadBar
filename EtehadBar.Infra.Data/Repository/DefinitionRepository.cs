using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class DefinitionRepository : IDefinitionRepository
    {
        private readonly ApplicationDbContext db;

        public DefinitionRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Definition obj)
        {
            db.Add(obj);
        }

        public IQueryable<Definition> Definitions()
        {
            return db.Definition.AsQueryable();
        }

        public void Delete(Definition obj)
        {
            db.Remove(obj);
        }

        public async Task<Definition> Get(long id)
        {
            return await db.Definition.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Definition obj)
        {
            db.Update(obj);
        }
    }
}

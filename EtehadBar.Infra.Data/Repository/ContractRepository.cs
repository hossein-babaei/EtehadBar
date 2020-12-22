using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext db;
        public ContractRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<Contract> Contracts()
        {
            return db.Contract.AsQueryable();
        }

        public void Create(Contract obj)
        {
            db.Add(obj);
        }

        public void Delete(Contract obj)
        {
            db.Remove(obj);
        }

        public async Task<Contract> Get(string id)
        {
            return await db.Contract.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Contract obj)
        {
            db.Update(obj);
        }
    }
}

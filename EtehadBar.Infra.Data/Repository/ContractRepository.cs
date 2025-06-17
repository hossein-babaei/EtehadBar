using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<Contract> Get(long id)
        {
            return await db.Contract.FindAsync(id);
        }

        public async Task<Contract> Get(string rowId)
        {
            return await db.Contract.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<List<long>> GetAllContractIdListForSameCustomer(long contractId)
        {
            return await db.Contract.AsNoTracking().Where(a =>
            a.CustomerId.Equals(db.Contract.AsNoTracking().Where(a => a.Id.Equals(contractId)).Select(a => a.CustomerId).FirstOrDefault())
            ).Select(a => a.Id).ToListAsync();
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

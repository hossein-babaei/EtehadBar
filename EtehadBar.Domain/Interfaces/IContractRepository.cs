using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IContractRepository
    {
        Task<Contract> Get(long id);
        Task<Contract> Get(string rowId);
        IQueryable<Contract> Contracts();
        Task<List<long>> GetAllContractIdListForSameCustomer(long contractId);
        void Update(Contract obj);
        void Create(Contract obj);
        void Delete(Contract obj);
        Task<int> Save();
    }
}

using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IContractRepository
    {
        Task<Contract> Get(long id);
        IQueryable<Contract> Contracts();
        void Update(Contract obj);
        void Create(Contract obj);
        void Delete(Contract obj);
        Task<int> Save();
    }
}

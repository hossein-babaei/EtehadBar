using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ITurnoverProfilePeriodRepository
    {
        Task<TurnoverProfilePeriod> Get(long id);
        Task<TurnoverProfilePeriod> Get(string rowId);
        IQueryable<TurnoverProfilePeriod> Query();
        void Update(TurnoverProfilePeriod obj);
        void Create(TurnoverProfilePeriod obj);
        void Delete(TurnoverProfilePeriod obj);
        Task<int> Save();
    }
}

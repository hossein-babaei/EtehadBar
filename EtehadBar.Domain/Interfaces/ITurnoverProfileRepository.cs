using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ITurnoverProfileRepository
    {
        Task<TurnoverProfile> Get(long id);
        Task<TurnoverProfile> Get(string rowId);
        IQueryable<TurnoverProfile> Query();
        void Update(TurnoverProfile obj);
        void Create(TurnoverProfile obj);
        void Delete(TurnoverProfile obj);
        Task<int> Save();
    }
}

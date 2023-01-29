using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ICostRepository
    {
        Task<Cost> Get(long id);
        IQueryable<Cost> Costs();
        void Update(Cost obj);
        void Create(Cost obj);
        void Delete(Cost obj);
        Task<int> Save();
    }
}

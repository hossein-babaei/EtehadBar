using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ITurnoverRepository
    {
        Task<Turnover> Get(long id);
        Task<Turnover> Get(string id);
        IQueryable<Turnover> Query();
        void Update(Turnover obj);
        void Create(Turnover obj);
        void Delete(Turnover obj);
        Task<int> Save();
    }
}

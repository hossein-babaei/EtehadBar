using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IDefinitionRepository
    {
        Task<Definition> Get(int id);
        IQueryable<Definition> Definitions();
        void Update(Definition obj);
        void Create(Definition obj);
        void Delete(Definition obj);
        Task<int> Save();
    }
}

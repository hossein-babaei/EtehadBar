using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IMehrcomParsCategoryRepository
    {
        Task<MehrcomParsCategory> Get(long id);
        Task<MehrcomParsCategory> Get(string id);
        IQueryable<MehrcomParsCategory> Categories();
        void Update(MehrcomParsCategory obj);
        void Create(MehrcomParsCategory obj);
        void Delete(MehrcomParsCategory obj);
        Task<int> Save();
    }
}

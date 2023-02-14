using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IDriverRepository
    {
        Task<Driver> Get(long id);
        Task<Driver> Get(string id);
        IQueryable<Driver> Drivers();
        void Update(Driver obj);
        void Create(Driver obj);
        void Delete(Driver obj);
        Task<int> Save();
    }
}

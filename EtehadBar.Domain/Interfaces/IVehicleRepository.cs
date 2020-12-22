using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle> Get(string id);
        IQueryable<Vehicle> Vehicles();
        void Update(Vehicle obj);
        void Create(Vehicle obj);
        Task<int> Save();
    }
}

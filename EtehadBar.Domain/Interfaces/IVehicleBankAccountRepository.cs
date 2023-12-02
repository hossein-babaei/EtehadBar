using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IVehicleBankAccountRepository
    {
        Task<VehicleBankAccount> Get(long id);
        Task<VehicleBankAccount> Get(string rowId);
        IQueryable<VehicleBankAccount> Query();
        void Update(VehicleBankAccount obj);
        Task Create(VehicleBankAccount obj);
        void Delete(VehicleBankAccount obj);
        Task<int> Save();
    }
}

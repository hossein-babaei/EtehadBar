using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IUserPlannerRepository
    {
        Task<UserPlanner> Get(long id);
        Task<EditUserPlannerVM> GetUserPlannerEditData(long id);
        IQueryable<UserPlanner> Query();
        void Update(UserPlanner obj);
        void UpdateRange(List<UserPlanner> list);
        void Create(UserPlanner obj);
        void Delete(UserPlanner obj);
        Task<int> Save();
    }
}

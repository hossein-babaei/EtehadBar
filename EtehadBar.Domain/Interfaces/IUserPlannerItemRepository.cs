using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IUserPlannerItemRepository
    {
        Task<UserPlannerItem> Get(long id);
        IQueryable<UserPlannerItem> Query();
        void Update(UserPlannerItem obj);
        void UpdateRange(List<UserPlannerItem> list);
        void Create(UserPlannerItem obj);
        void Delete(UserPlannerItem obj);
        Task<int> Save();
    }
}

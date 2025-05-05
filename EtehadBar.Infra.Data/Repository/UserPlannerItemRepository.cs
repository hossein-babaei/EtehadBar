using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class UserPlannerItemRepository : IUserPlannerItemRepository
    {
        private readonly ApplicationDbContext db;
        public UserPlannerItemRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<UserPlannerItem> Query()
        {
            return db.UserPlannerItem.AsQueryable();
        }

        public void Create(UserPlannerItem obj)
        {
            db.Add(obj);
        }

        public void Delete(UserPlannerItem obj)
        {
            db.Remove(obj);
        }

        public async Task<UserPlannerItem> Get(long id)
        {
            return await db.UserPlannerItem.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(UserPlannerItem obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<UserPlannerItem> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }
    }
}

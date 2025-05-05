using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using MD.PersianDateTime.Standard;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class UserPlannerRepository : IUserPlannerRepository
    {
        private readonly ApplicationDbContext db;
        public UserPlannerRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<UserPlanner> Query()
        {
            return db.UserPlanner.AsQueryable();
        }

        public void Create(UserPlanner obj)
        {
            db.Add(obj);
        }

        public void Delete(UserPlanner obj)
        {
            db.Remove(obj);
        }

        public async Task<UserPlanner> Get(long id)
        {
            return await db.UserPlanner.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(UserPlanner obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<UserPlanner> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }

        public async Task<EditUserPlannerVM> GetUserPlannerEditData(long id)
        {
            var item = await db.UserPlanner.Where(a => a.Id.Equals(id)).FirstOrDefaultAsync();
            var date = new PersianDateTime(item.Date);

            return new EditUserPlannerVM
            {
                Id = item.Id,
                Day = date.Day,
                Month = date.Month,
                Year = date.Year
            };
        }
    }
}

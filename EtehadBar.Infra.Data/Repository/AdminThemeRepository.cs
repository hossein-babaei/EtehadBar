using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class AdminThemeRepository : IAdminThemeRepository
    {
        private readonly ApplicationDbContext db;

        public AdminThemeRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(AdminTheme obj)
        {
            db.Add(obj);
        }

        public async Task<AdminTheme> GetByUserId(string userId)
        {
            return await db.AdminTheme.AsNoTracking().SingleOrDefaultAsync(a => a.UserId.Equals(userId));
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }
    }
}

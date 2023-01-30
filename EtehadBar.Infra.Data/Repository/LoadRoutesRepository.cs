using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class LoadRoutesRepository : ILoadRoutesRepository
    {
        private readonly ApplicationDbContext db;

        public LoadRoutesRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<bool> CheckNameExist(string title)
        {
            return await db.LoadRoute.AsNoTracking().AnyAsync(a => a.Title.Equals(title));
        }

        public void Create(LoadRoutes obj)
        {
            db.Add(obj);
        }

        public void Delete(LoadRoutes obj)
        {
            db.Remove(obj);
        }

        public async Task<LoadRoutes> Get(long id)
        {
            return await db.LoadRoute.FindAsync(id);
        }

        public IQueryable<LoadRoutes> LoadRoutes()
        {
            return db.LoadRoute.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(LoadRoutes obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<LoadRoutes> list)
        {
            db.UpdateRange(list);
        }
    }
}

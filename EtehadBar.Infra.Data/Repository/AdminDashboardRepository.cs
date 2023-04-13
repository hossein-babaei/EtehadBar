using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly ApplicationDbContext db;

        public AdminDashboardRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<AdminDashboardVM> GetAdminData(int? dayLimit)
        {
            var limit = dayLimit.HasValue ? DateTime.Now.AddDays(-dayLimit.Value) : DateTime.Now.AddDays(-1);
            var loadFactors = await db.LoadFactor.Where(a => a.CreateDateTime >= limit).ToListAsync();

            foreach (var item in loadFactors.Where(a => a.Tonnage.HasValue))
            {
                item.Amount += item.Tonnage.Value * item.TonnagePrice.Value;
                item.DriverFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;
            }

            foreach (var item in loadFactors.Where(a => a.LoadSleepPrice.HasValue || a.WeighbridgePrice.HasValue))
            {
                if (item.LoadSleepPrice.HasValue)
                {
                    item.Amount += item.LoadSleepPrice.Value;
                }
                if (item.DriverLoadSleepPrice.HasValue)
                {
                    item.DriverFee += item.DriverLoadSleepPrice.Value;
                }
                if (item.WeighbridgePrice.HasValue)
                {
                    item.Amount += item.WeighbridgePrice.Value;
                    item.DriverFee += item.WeighbridgePrice.Value;
                }
            }

            var data = new AdminDashboardVM()
            {
                LoadFactorsAmount = loadFactors.Sum(a => a.Amount),
                LoadFactorsDriverFee = loadFactors.Sum(a => a.DriverFee),
                RegisteredLoadFactorCount = loadFactors.Count,
                CostAmount = await db.Cost.Where(a => a.Date >= limit).SumAsync(a => a.Amount),
                PaymentAmount = await db.Payment.Where(a => a.Date >= limit).SumAsync(a => a.Amount),
                SaipaPlascoAmount = loadFactors.Where(a => a.SaipaPlascoLoadFactor is not null).Sum(a => a.Amount),
                SaipaPlascoDriverFee = loadFactors.Where(a => a.SaipaPlascoLoadFactor is not null).Sum(a => a.DriverFee),
                SaipaPressAmount = loadFactors.Where(a => a.SaipaPressLoadFactor is not null).Sum(a => a.Amount),
                SaipaPressDeriverFee = loadFactors.Where(a => a.SaipaPressLoadFactor is not null).Sum(a => a.DriverFee),
                SazehGostarAmount = loadFactors.Where(a => a.SazehGostarLoadFactor is not null).Sum(a => a.Amount),
                SazehGostarDriverFee = loadFactors.Where(a => a.SazehGostarLoadFactor is not null).Sum(a => a.DriverFee),
                MehrcomParsAmount = loadFactors.Where(a => a.MehrcomParsLoadFactor is not null).Sum(a => a.Amount),
                MehrcomParsDriverFee = loadFactors.Where(a => a.MehrcomParsLoadFactor is not null).Sum(a => a.DriverFee),
                UserActivity = new List<AdminDashboardUserActivityBoxVM>()
            };

            var registerUsers = await db.Users.Where(a => a.Role == ApplicationRoleType.RegisterUser && a.Status).AsNoTracking()
                .Select(a => new
                {
                    a.Id,
                    a.Firstname,
                    a.Lastname,
                    a.Avatar
                }).ToListAsync();

            foreach (var user in registerUsers.OrderBy(a => a.Lastname))
            {
                data.UserActivity.Add(new AdminDashboardUserActivityBoxVM
                {
                    Avatar = user.Avatar,
                    Fullname = user.Firstname + " " + user.Lastname,
                    UserId = user.Id,
                    LoadFactorRegisterdCount = loadFactors.Count(a => a.AdminId.Equals(user.Id))
                });
            }

            return data;
        }

        public async Task<AdminDashboardUserActivityBoxVM> GetRegisterUserData(string userId, int? dayLimit)
        {
            var now = DateTime.Now;
            var limit = dayLimit.HasValue ? DateTime.Now.AddDays(-dayLimit.Value) : new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var user = await db.Users.FindAsync(userId);
            return new AdminDashboardUserActivityBoxVM
            {
                Avatar = user.Avatar,
                Fullname = user.Firstname + " " + user.Lastname,
                UserId = user.Id,
                LoadFactorRegisterdCount = await db.LoadFactor.Where(a => a.CreateDateTime >= limit && a.AdminId.Equals(userId)).AsNoTracking().CountAsync()
            };
        }

        public async Task<List<AdminDashboardUserActivityBoxVM>> GetUserData(int? dayLimit)
        {
            var data = new List<AdminDashboardUserActivityBoxVM>();
            var limit = dayLimit.HasValue ? DateTime.Now.AddDays(-dayLimit.Value) : DateTime.Now.AddDays(-1);
            var registerUsers = await db.Users.Where(a => a.Role == ApplicationRoleType.RegisterUser && a.Status).AsNoTracking()
                .Select(a => new
                {
                    a.Id,
                    a.Firstname,
                    a.Lastname,
                    a.Avatar
                }).ToListAsync();

            var loadFactors = await db.LoadFactor.Where(a => a.CreateDateTime >= limit).AsNoTracking().Select(a => a.AdminId).ToListAsync();

            foreach (var user in registerUsers.OrderBy(a => a.Lastname))
            {
                data.Add(new AdminDashboardUserActivityBoxVM
                {
                    Avatar = user.Avatar,
                    Fullname = user.Firstname + " " + user.Lastname,
                    UserId = user.Id,
                    LoadFactorRegisterdCount = loadFactors.Count(a => a.Equals(user.Id))
                });
            }
            return data;
        }
    }
}

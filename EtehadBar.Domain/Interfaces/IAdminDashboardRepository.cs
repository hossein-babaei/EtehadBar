using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IAdminDashboardRepository
    {
        public Task<AdminDashboardVM> GetAdminData(int? dayLimit);
        public Task<List<AdminDashboardUserActivityBoxVM>> GetUserData(int? dayLimit);
        public Task<AdminDashboardUserActivityBoxVM> GetRegisterUserData(string userId, int? dayLimit);
    }
}

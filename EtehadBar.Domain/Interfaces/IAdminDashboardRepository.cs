using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IAdminDashboardRepository
    {
        public Task<AdminDashboardVM> GetAdminData(int? dayLimit);
        public Task<AdminDashboardUserActivityBoxVM> GetRegisterUserData(int? dayLimit);
    }
}

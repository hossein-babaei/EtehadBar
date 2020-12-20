using EtehadBar.Domain.Models;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IAdminThemeRepository
    {
        Task<AdminTheme> GetByUserId(string userId);
        void Create(AdminTheme obj);
        Task<int> Save();
        void Update(AdminTheme obj);
    }
}

using EtehadBar.Domain.Models;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IConfigRepository
    {
        Task<string> CurrentYear();
        Task<Config> First();
        Task<int> Save();
        void Update(Config obj);
    }
}

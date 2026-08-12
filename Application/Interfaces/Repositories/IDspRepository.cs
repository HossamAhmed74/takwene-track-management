using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDspRepository
    {
        Task<bool> ExistsAsync(int id);
    }
}
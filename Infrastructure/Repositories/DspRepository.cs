using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DspRepository : IDspRepository
    {
        private readonly AppDbContext _context;

        public DspRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Dsps.AsNoTracking().AnyAsync(d => d.Id == id);
        }
    }
}
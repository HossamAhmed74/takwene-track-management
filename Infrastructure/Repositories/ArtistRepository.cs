using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly AppDbContext _context;
        public ArtistRepository(AppDbContext context) => _context = context;

        public async Task<Artist?> GetByIdAsync(int id) =>
            await _context.Artists.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IReadOnlyList<Artist>> GetAllAsync() =>
            await _context.Artists.AsNoTracking().OrderBy(a => a.Name).ToListAsync();

        public async Task AddAsync(Artist artist)
        {
            await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();
        }
    }
}
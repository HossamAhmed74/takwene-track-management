using Application.DTOs.Tracks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly AppDbContext _context;
        public TrackRepository(AppDbContext context) => _context = context;

        public async Task<Track?> GetByIdAsync(int id) =>
            await _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Distributions).ThenInclude(d => d.Dsp)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IReadOnlyList<Track>> SearchAsync(TrackQueryDto query)
        {
            IQueryable<Track> q = _context.Tracks.Include(t => t.Artist).AsNoTracking();

            if (query.ArtistId.HasValue) q = q.Where(t => t.ArtistId == query.ArtistId.Value);
            if (!string.IsNullOrWhiteSpace(query.Genre)) q = q.Where(t => t.Genre.ToLower() == query.Genre.Trim().ToLower());
            if (query.Status.HasValue) q = q.Where(t => t.Status == query.Status.Value);

            return await q.OrderByDescending(t => t.ReleaseDate).ToListAsync();
        }

        public async Task AddAsync(Track track)
        {
            await _context.Tracks.AddAsync(track);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Track track)
        {
            if (_context.Entry(track).State == EntityState.Detached) _context.Tracks.Attach(track);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsrcExistsAsync(string isrc) =>
            await _context.Tracks.AsNoTracking().AnyAsync(t => t.Isrc == isrc);

        public async Task<bool> ArtistExistsAsync(int artistId) =>
            await _context.Artists.AsNoTracking().AnyAsync(a => a.Id == artistId);

        public async Task<bool> DspExistsAsync(int dspId) =>
            await _context.Dsps.AsNoTracking().AnyAsync(d => d.Id == dspId);
    }
}
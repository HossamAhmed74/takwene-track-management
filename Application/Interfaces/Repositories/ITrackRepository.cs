using Application.DTOs.Tracks;
using Domain.Entities;

namespace Application.Interfaces;

public interface ITrackRepository
{
    Task<Track?> GetByIdAsync(int id);
    Task<IReadOnlyList<Track>> SearchAsync(TrackQueryDto query);
    Task AddAsync(Track track);
    Task UpdateAsync(Track track);
    Task<bool> IsrcExistsAsync(string isrc);
    Task<bool> ArtistExistsAsync(int artistId);
    Task<bool> DspExistsAsync(int dspId);
}
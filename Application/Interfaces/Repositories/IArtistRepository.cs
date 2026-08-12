using Domain.Entities;

namespace Application.Interfaces;

public interface IArtistRepository
{
    Task<Artist?> GetByIdAsync(int id);
    Task<IReadOnlyList<Artist>> GetAllAsync();
    Task AddAsync(Artist artist);
}
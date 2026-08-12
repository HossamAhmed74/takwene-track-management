using Application.DTOs.Artist;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ArtistService
{
    private readonly IArtistRepository _artistRepository;

    public ArtistService(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<ArtistResponseDto> CreateAsync(
        ArtistCreateDto request
    )
    {
        var artist = new Artist
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Country = request.Country.Trim()
        };

        await _artistRepository.AddAsync(artist);

        return new ArtistResponseDto
        {
            Id = artist.Id,
            Name = artist.Name,
            Email = artist.Email,
            Country = artist.Country
        };
    }

    public async Task<IReadOnlyList<ArtistResponseDto>> GetAllAsync()
    {
        var artists = await _artistRepository.GetAllAsync();

        return artists
            .Select(artist => new ArtistResponseDto
            {
                Id = artist.Id,
                Name = artist.Name,
                Email = artist.Email,
                Country = artist.Country
            })
            .ToList();
    }
}
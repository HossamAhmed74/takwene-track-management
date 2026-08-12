using Application.DTOs.Artist;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistsController : ControllerBase
{
    private readonly ArtistService _artistService;

    public ArtistsController(ArtistService artistService)
    {
        _artistService = artistService;
    }

    [HttpPost("CreateArtist")]
    public async Task<ActionResult<ArtistResponseDto>> Create(
        [FromBody] ArtistCreateDto request
    )
    {
        var response = await _artistService.CreateAsync(request);
        return Ok(response);
    }

    [HttpGet("GetAllArtists")]
    public async Task<ActionResult<IReadOnlyList<ArtistResponseDto>>> GetAll()
    {
        var response = await _artistService.GetAllAsync();
        return Ok(response);
    }
}
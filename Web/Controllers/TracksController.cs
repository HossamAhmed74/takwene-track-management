using Application.DTOs.TrackDistribution;
using Application.DTOs.Tracks;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/tracks")]
public class TracksController : ControllerBase
{
    private readonly TrackService _trackService;

    public TracksController(TrackService trackService)
    {
        _trackService = trackService;
    }

    [HttpPost("CreateTrack")]
    public async Task<ActionResult<TrackDetailResponse>> Create(
        [FromBody] TrackCreateDto request
    )
    {
        var response = await _trackService.CreateAsync(request);
        return Created($"/api/tracks/{response.Id}", response);
    }

    [Authorize]
    [HttpGet("GetAllTracks")]
    public async Task<ActionResult<IReadOnlyList<TrackListItemResponseDto>>> GetTracks(
        [FromQuery] TrackQueryDto query
    )
    {
        var response = await _trackService.SearchAsync(query);
        return Ok(response);
    }

    [HttpGet("GetTrackById/{id}")]
    public async Task<ActionResult<TrackDetailResponse>> GetTrackById(int id)
    {
        var response = await _trackService.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpPost("{id}/distribute")]
    public async Task<ActionResult<TrackDetailResponse>> Distribute(int id,[FromBody] TrackDistributionCreateDto request)
    {
        var response = await _trackService.DistributeAsync(id, request.DspIds);
        return Ok(response);
    }

    [Authorize]
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TrackDetailResponse>> UpdateStatus(
        int id,
        [FromBody] UpdateTrackStatusDto request
    )
    {
        var response = await _trackService.UpdateStatusAsync(id, request.Status);
        return Ok(response);
    }
}
using Application.DTOs.TrackDistribution;
using Application.DTOs.Tracks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services;

public class TrackService
{
    private readonly ITrackRepository _trackRepository;
    private readonly IDspRepository _dspRepository;

    public TrackService(ITrackRepository trackRepository, IDspRepository dspRepository)
    {
        _trackRepository = trackRepository;
        _dspRepository = dspRepository;
    }

    public async Task<TrackDetailResponse> CreateAsync(TrackCreateDto request)
    {
        var artistExists = await _trackRepository.ArtistExistsAsync(request.ArtistId);

        if (!artistExists)
        {
            throw new NotFoundException($"Artist with id {request.ArtistId} was not found.");
        }

        var isrc = NormalizeIsrc(request.Isrc);

        var isrcExists = await _trackRepository.IsrcExistsAsync(isrc);

        if (isrcExists)
        {
            throw new InvalidDataException($"ISRC '{isrc}' already exists.");
        }

        var track = new Track
        {
            ArtistId = request.ArtistId,
            Title = request.Title.Trim(),
            Isrc = isrc,
            ReleaseDate = DateTime.SpecifyKind(request.ReleaseDate, DateTimeKind.Utc),
            Genre = request.Genre.Trim(),
            Status = request.Status ?? TrackStatus.Drafted,
            CreatedAt = DateTime.UtcNow,
        };

        await _trackRepository.AddAsync(track);

        return await GetByIdAsync(track.Id);
    }

    public async Task<IReadOnlyList<TrackListItemResponseDto>> SearchAsync(TrackQueryDto query)
    {
        var tracks = await _trackRepository.SearchAsync(query);

        return tracks
            .Select(track => new TrackListItemResponseDto
            {
                Id = track.Id,
                Title = track.Title,
                ArtistId = track.ArtistId,
                ArtistName = track.Artist?.Name ?? string.Empty,
                Isrc = track.Isrc,
                ReleaseDate = track.ReleaseDate,
                Genre = track.Genre,
                Status = MapTrackStatus(track.Status)
            })
            .ToList();
    }

    public async Task<TrackDetailResponse> GetByIdAsync(int trackId)
    {
        var track = await _trackRepository.GetByIdAsync(trackId);

        if (track is null)
        {
            throw new NotFoundException($"Track with id {trackId} was not found.");
        }

        return MapTrackDetail(track);
    }

    public async Task<TrackDetailResponse> DistributeAsync(int trackId, IEnumerable<int> dspIds)
    {
        var track = await _trackRepository.GetByIdAsync(trackId);
        if (track is null) throw new NotFoundException($"Track {trackId} not found.");

        if (track.Status == TrackStatus.Drafted)
            throw new InvalidDataException("A drafted track cannot be distributed. Change status to Submitted first.");

        foreach (var dspId in dspIds.Distinct())
        {
            if (!await _dspRepository.ExistsAsync(dspId))
                throw new NotFoundException($"DSP with id {dspId} not found.");

            if (!track.Distributions.Any(d => d.DspId == dspId))
            {
                track.Distributions.Add(new TrackDistribution
                {
                    TrackId = track.Id,
                    DspId = dspId,
                    SubmittedAt = System.DateTime.UtcNow,
                    Status = DistributionStatus.Pending
                });
            }
        }

        await _trackRepository.UpdateAsync(track);
        return await GetByIdAsync(track.Id);
    }

    public async Task<TrackDetailResponse> UpdateStatusAsync(int trackId, TrackStatus status)
    {
        var track = await _trackRepository.GetByIdAsync(trackId);

        if (track is null)
        {
            throw new NotFoundException($"Track with id {trackId} was not found.");
        }

        track.Status = status;

        await _trackRepository.UpdateAsync(track);

        return MapTrackDetail(track);
    }

    private static string NormalizeIsrc(string isrc)
    {
        return (isrc ?? string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToUpperInvariant();
    }

    private static TrackDetailResponse MapTrackDetail(Track track)
    {
        return new TrackDetailResponse
        {
            Id = track.Id,
            Title = track.Title,
            ArtistId = track.ArtistId,
            ArtistName = track.Artist?.Name ?? string.Empty,
            Isrc = track.Isrc,
            ReleaseDate = track.ReleaseDate,
            Genre = track.Genre,
            Status = MapTrackStatus(track.Status),
            Distributions = track.Distributions
                .OrderBy(distribution => distribution.Dsp?.Name ?? string.Empty)
                .Select(distribution => new TrackDistributionResponseDto
                {
                    Id = distribution.Id,
                    TrackId = distribution.TrackId,
                    TrackTitle = distribution.Track?.Title ?? string.Empty,
                    DspId = distribution.DspId,
                    DspName = distribution.Dsp?.Name ?? string.Empty,
                    SubmittedAt = distribution.SubmittedAt,
                    Status = MapTrackDistribution(distribution.Status)
                })
                .ToList()
        };
    }

    private static string MapTrackStatus(TrackStatus status)
    {
        return status switch
        {
            TrackStatus.Drafted => "draft",
            TrackStatus.Submitted => "submitted",
            TrackStatus.Distributed => "distributed",
            _ => "unknown"
        };
    }

    private static string MapTrackDistribution(DistributionStatus status)
    {
        return status switch
        {
            DistributionStatus.Pending => "pending",
            DistributionStatus.Live => "live",
            DistributionStatus.Rejected => "rejected",
            _ => "unknown"
        };
    }
}
using Application.DTOs.TrackDistribution;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tracks
{
    public class TrackDetailResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string Isrc { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public IReadOnlyList<TrackDistributionResponseDto> Distributions { get; set; }
            = new List<TrackDistributionResponseDto>();
    }
}

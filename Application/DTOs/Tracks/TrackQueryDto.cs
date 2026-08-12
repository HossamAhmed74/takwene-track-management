using Domain.Enums;

namespace Application.DTOs.Tracks
{
    public class TrackQueryDto
    {
        public int? ArtistId { get; set; }
        public string? Genre { get; set; }
        public TrackStatus? Status { get; set; }
    }
}
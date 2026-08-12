using Domain.Enums;

namespace Application.DTOs.Tracks
{
    public class TrackResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string Isrc { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public TrackStatus Status { get; set; }
    }
}
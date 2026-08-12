using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Track : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public Artist? Artist { get; set; }
    public string Isrc { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; } = string.Empty;
    public TrackStatus Status { get; set; } = TrackStatus.Drafted;
    public ICollection<TrackDistribution> Distributions { get; set; } = new List<TrackDistribution>();
}
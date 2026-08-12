using Domain.Common;
using Domain.Entities;

namespace Domain.Entities
{
    public class Dsp : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<TrackDistribution> TrackDistributions { get; set; } = new List<TrackDistribution>();
    }
}

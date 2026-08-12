using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class TrackDistribution : BaseEntity
    {
        public int TrackId { get; set; }
        public Track Track { get; set; } = null!;

        public int DspId { get; set; }
        public Dsp Dsp { get; set; } = null!;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DistributionStatus Status { get; set; } = DistributionStatus.Pending;
    }
}
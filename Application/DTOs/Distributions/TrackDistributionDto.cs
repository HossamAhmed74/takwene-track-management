using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.TrackDistribution
{
    public class TrackDistributionCreateDto
    {
        public List<int> DspIds { get; set; } = new();
    }

    public class TrackDistributionResponseDto
    {
        public int Id { get; set; }
        public int TrackId { get; set; }
        public string TrackTitle { get; set; } = string.Empty;
        public int DspId { get; set; }
        public string DspName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
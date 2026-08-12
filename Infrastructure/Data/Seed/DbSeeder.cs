using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Only seed if the database is empty — avoids duplicate data on every startup
            if (await context.Artists.AnyAsync())
                return;

            // --- Artists (3+) ---
            var artists = new List<Artist>
            {
                new Artist { Name = "Luna Ray",      Email = "luna.ray@example.com",      Country = "USA" },
                new Artist { Name = "Kofi Boateng",   Email = "kofi.boateng@example.com",  Country = "Ghana" },
                new Artist { Name = "Elena Marquez",  Email = "elena.marquez@example.com", Country = "Spain" },
                new Artist { Name = "Yusuf Demir",    Email = "yusuf.demir@example.com",   Country = "Turkey" }
            };

            await context.Artists.AddRangeAsync(artists);
            await context.SaveChangesAsync();

            // --- DSPs (exactly 3) ---
            var dsps = new List<Dsp>
            {
                new Dsp { Name = "Spotify" },
                new Dsp { Name = "Apple Music" },
                new Dsp { Name = "YouTube" }
            };

            await context.Dsps.AddRangeAsync(dsps);
            await context.SaveChangesAsync();

            // --- Tracks (8+, across different genres and statuses) ---
            var tracks = new List<Track>
            {
                new Track
                {
                    Title = "Midnight Drive",
                    ArtistId = artists[0].Id,
                    Isrc = "US-S1Z-24-00001",
                    ReleaseDate = new DateTime(2024, 3, 10),
                    Genre = "Pop",
                    Status = TrackStatus.Drafted
                },
                new Track
                {
                    Title = "Golden Hour",
                    ArtistId = artists[0].Id,
                    Isrc = "US-S1Z-24-00002",
                    ReleaseDate = new DateTime(2024, 5, 22),
                    Genre = "Pop",
                    Status = TrackStatus.Submitted
                },
                new Track
                {
                    Title = "Accra Nights",
                    ArtistId = artists[1].Id,
                    Isrc = "GH-K2B-23-00003",
                    ReleaseDate = new DateTime(2023, 11, 1),
                    Genre = "Afrobeat",
                    Status = TrackStatus.Distributed
                },
                new Track
                {
                    Title = "Rhythm of Ghana",
                    ArtistId = artists[1].Id,
                    Isrc = "GH-K2B-24-00004",
                    ReleaseDate = new DateTime(2024, 1, 15),
                    Genre = "Afrobeat",
                    Status = TrackStatus.Drafted
                },
                new Track
                {
                    Title = "Flamenco Dreams",
                    ArtistId = artists[2].Id,
                    Isrc = "ES-EM3-23-00005",
                    ReleaseDate = new DateTime(2023, 8, 9),
                    Genre = "Flamenco",
                    Status = TrackStatus.Distributed
                },
                new Track
                {
                    Title = "Barcelona Skyline",
                    ArtistId = artists[2].Id,
                    Isrc = "ES-EM3-24-00006",
                    ReleaseDate = new DateTime(2024, 6, 30),
                    Genre = "Electronic",
                    Status = TrackStatus.Submitted
                },
                new Track
                {
                    Title = "Bosphorus Wind",
                    ArtistId = artists[3].Id,
                    Isrc = "TR-YD4-23-00007",
                    ReleaseDate = new DateTime(2023, 12, 5),
                    Genre = "Electronic",
                    Status = TrackStatus.Drafted
                },
                new Track
                {
                    Title = "Istanbul Nights",
                    ArtistId = artists[3].Id,
                    Isrc = "TR-YD4-24-00008",
                    ReleaseDate = new DateTime(2024, 2, 14),
                    Genre = "Hip-Hop",
                    Status = TrackStatus.Distributed
                },
                new Track
                {
                    Title = "Silent Echoes",
                    ArtistId = artists[0].Id,
                    Isrc = "US-S1Z-24-00009",
                    ReleaseDate = new DateTime(2024, 7, 1),
                    Genre = "Hip-Hop",
                    Status = TrackStatus.Submitted
                }
            };

            await context.Tracks.AddRangeAsync(tracks);
            await context.SaveChangesAsync();

            // --- Track Distributions (only for tracks with Status = Distributed) ---
            var distributedTracks = tracks.Where(t => t.Status == TrackStatus.Distributed).ToList();

            var distributions = new List<TrackDistribution>();

            foreach (var track in distributedTracks)
            {
                distributions.Add(new TrackDistribution
                {
                    TrackId = track.Id,
                    DspId = dsps[0].Id, // Spotify
                    SubmittedAt = track.ReleaseDate.AddDays(-3),
                    Status = DistributionStatus.Live
                });

                distributions.Add(new TrackDistribution
                {
                    TrackId = track.Id,
                    DspId = dsps[1].Id, // Apple Music
                    SubmittedAt = track.ReleaseDate.AddDays(-2),
                    Status = DistributionStatus.Live
                });
            }

            // Give one Submitted track a "Pending" distribution too, for status variety
            var submittedTrack = tracks.First(t => t.Status == TrackStatus.Submitted);
            distributions.Add(new TrackDistribution
            {
                TrackId = submittedTrack.Id,
                DspId = dsps[2].Id, // YouTube
                SubmittedAt = DateTime.UtcNow.AddDays(-1),
                Status = DistributionStatus.Pending
            });

            // And one Rejected example, for full status coverage
            distributions.Add(new TrackDistribution
            {
                TrackId = distributedTracks[0].Id,
                DspId = dsps[2].Id, // YouTube
                SubmittedAt = distributedTracks[0].ReleaseDate.AddDays(-1),
                Status = DistributionStatus.Rejected
            });

            await context.TrackDistributions.AddRangeAsync(distributions);
            await context.SaveChangesAsync();
        }
    }
}
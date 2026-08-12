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
            if (!await context.Artists.AnyAsync())
            {
                var artists = new List<Artist>
                {
                    new Artist { Name = "Luna Ray",      Email = "luna.ray@example.com",      Country = "USA" },
                    new Artist { Name = "Kofi Boateng",   Email = "kofi.boateng@example.com",  Country = "Ghana" },
                    new Artist { Name = "Elena Marquez",  Email = "elena.marquez@example.com", Country = "Spain" },
                    new Artist { Name = "Yusuf Demir",    Email = "yusuf.demir@example.com",   Country = "Turkey" }
                };

                await context.Artists.AddRangeAsync(artists);
                await context.SaveChangesAsync();
            }

            // Fetch from DB so dependent tables (Tracks) can use their IDs
            var allArtists = await context.Artists.ToListAsync();


            if (!await context.Dsps.AnyAsync())
            {
                var dsps = new List<Dsp>
                {
                    new Dsp { Name = "Spotify" },
                    new Dsp { Name = "Apple Music" },
                    new Dsp { Name = "YouTube" }
                };

                await context.Dsps.AddRangeAsync(dsps);
                await context.SaveChangesAsync();
            }

            // Fetch from DB so dependent tables (Distributions) can use their IDs
            var allDsps = await context.Dsps.ToListAsync();


            if (!await context.Tracks.AnyAsync())
            {
                var tracks = new List<Track>
                {
                    new Track { Title = "Midnight Drive",    ArtistId = allArtists[0].Id, Isrc = "US-S1Z-24-00001", ReleaseDate = new DateTime(2024, 3, 10), Genre = "Pop",       Status = TrackStatus.Drafted },
                    new Track { Title = "Golden Hour",       ArtistId = allArtists[0].Id, Isrc = "US-S1Z-24-00002", ReleaseDate = new DateTime(2024, 5, 22), Genre = "Pop",       Status = TrackStatus.Submitted },
                    new Track { Title = "Accra Nights",      ArtistId = allArtists[1].Id, Isrc = "GH-K2B-23-00003", ReleaseDate = new DateTime(2023, 11, 1), Genre = "Afrobeat",  Status = TrackStatus.Distributed },
                    new Track { Title = "Rhythm of Ghana",   ArtistId = allArtists[1].Id, Isrc = "GH-K2B-24-00004", ReleaseDate = new DateTime(2024, 1, 15), Genre = "Afrobeat",  Status = TrackStatus.Drafted },
                    new Track { Title = "Flamenco Dreams",   ArtistId = allArtists[2].Id, Isrc = "ES-EM3-23-00005", ReleaseDate = new DateTime(2023, 8, 9),  Genre = "Flamenco", Status = TrackStatus.Distributed },
                    new Track { Title = "Barcelona Skyline", ArtistId = allArtists[2].Id, Isrc = "ES-EM3-24-00006", ReleaseDate = new DateTime(2024, 6, 30), Genre = "Electronic",Status = TrackStatus.Submitted },
                    new Track { Title = "Bosphorus Wind",    ArtistId = allArtists[3].Id, Isrc = "TR-YD4-23-00007", ReleaseDate = new DateTime(2023, 12, 5), Genre = "Electronic",Status = TrackStatus.Drafted },
                    new Track { Title = "Istanbul Nights",   ArtistId = allArtists[3].Id, Isrc = "TR-YD4-24-00008", ReleaseDate = new DateTime(2024, 2, 14), Genre = "Hip-Hop",   Status = TrackStatus.Distributed },
                    new Track { Title = "Silent Echoes",     ArtistId = allArtists[0].Id, Isrc = "US-S1Z-24-00009", ReleaseDate = new DateTime(2024, 7, 1),  Genre = "Hip-Hop",   Status = TrackStatus.Submitted }
                };

                await context.Tracks.AddRangeAsync(tracks);
                await context.SaveChangesAsync();
            }

            // Fetch from DB so dependent tables (Distributions) can use their IDs
            var allTracks = await context.Tracks.ToListAsync();


            if (!await context.TrackDistributions.AnyAsync())
            {
                var distributedTracks = allTracks.Where(t => t.Status == TrackStatus.Distributed).ToList();
                var submittedTrack = allTracks.First(t => t.Status == TrackStatus.Submitted);

                var distributions = new List<TrackDistribution>();

                // Distributions for Distributed tracks
                foreach (var track in distributedTracks)
                {
                    distributions.Add(new TrackDistribution
                    {
                        TrackId = track.Id,
                        DspId = allDsps[0].Id, // Spotify
                        SubmittedAt = track.ReleaseDate.AddDays(-3),
                        Status = DistributionStatus.Live
                    });

                    distributions.Add(new TrackDistribution
                    {
                        TrackId = track.Id,
                        DspId = allDsps[1].Id, // Apple Music
                        SubmittedAt = track.ReleaseDate.AddDays(-2),
                        Status = DistributionStatus.Live
                    });
                }

                // Give one Submitted track a "Pending" distribution
                distributions.Add(new TrackDistribution
                {
                    TrackId = submittedTrack.Id,
                    DspId = allDsps[2].Id, // YouTube
                    SubmittedAt = DateTime.UtcNow.AddDays(-1),
                    Status = DistributionStatus.Pending
                });

                // And one Rejected example
                distributions.Add(new TrackDistribution
                {
                    TrackId = distributedTracks[0].Id,
                    DspId = allDsps[2].Id, // YouTube
                    SubmittedAt = distributedTracks[0].ReleaseDate.AddDays(-1),
                    Status = DistributionStatus.Rejected
                });

                await context.TrackDistributions.AddRangeAsync(distributions);
                await context.SaveChangesAsync();
            }
        }
    }
}
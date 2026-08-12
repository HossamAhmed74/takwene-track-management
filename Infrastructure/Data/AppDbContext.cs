using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<Dsp> Dsps => Set<Dsp>();
        public DbSet<TrackDistribution> TrackDistributions => Set<TrackDistribution>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations (once you create them in Persistence/Configurations)
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
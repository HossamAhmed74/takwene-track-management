using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            // Table Name
            builder.ToTable("Tracks");

            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(t => t.Isrc)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(t => t.ReleaseDate)
                .IsRequired();

            builder.Property(t => t.Genre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(TrackStatus.Drafted);

            // Indexes
            builder.HasIndex(t => t.Isrc)
                .IsUnique()
                .HasDatabaseName("IX_Tracks_Isrc");

            builder.HasIndex(t => t.ArtistId)
                .HasDatabaseName("IX_Tracks_ArtistId");

            builder.HasIndex(t => t.Genre)
                .HasDatabaseName("IX_Tracks_Genre");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Tracks_Status");

            // Composite index for filtering (artistId + genre + status)
            builder.HasIndex(t => new { t.ArtistId, t.Genre, t.Status })
                .HasDatabaseName("IX_Tracks_ArtistId_Genre_Status");

            // Relationships
            builder.HasOne(t => t.Artist)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Distributions)
                .WithOne(d => d.Track)
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
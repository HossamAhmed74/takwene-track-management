using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
    {
        public void Configure(EntityTypeBuilder<Artist> builder)
        {
            // Table Name
            builder.ToTable("Artists");

            // Primary Key
            builder.HasKey(a => a.Id);

            // Properties
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(320);

            builder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(a => a.Email)
                .IsUnique()
                .HasDatabaseName("IX_Artists_Email");

            builder.HasIndex(a => a.Name)
                .HasDatabaseName("IX_Artists_Name");

            // Relationships
            builder.HasMany(a => a.Tracks)
                .WithOne(t => t.Artist)
                .HasForeignKey(t => t.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
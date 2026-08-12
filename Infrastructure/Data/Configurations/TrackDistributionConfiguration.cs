using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class TrackDistributionConfiguration : IEntityTypeConfiguration<TrackDistribution>
    {
        public void Configure(EntityTypeBuilder<TrackDistribution> builder)
        {
            // Table Name
            builder.ToTable("TrackDistributions");

            // Primary Key
            builder.HasKey(td => td.Id);

            // Properties
            builder.Property(td => td.SubmittedAt)
                .IsRequired();

            builder.Property(td => td.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(DistributionStatus.Pending);

            // Indexes
            builder.HasIndex(td => td.TrackId)
                .HasDatabaseName("IX_TrackDistributions_TrackId");

            builder.HasIndex(td => td.DspId)
                .HasDatabaseName("IX_TrackDistributions_DspId");

            builder.HasIndex(td => td.Status)
                .HasDatabaseName("IX_TrackDistributions_Status");

            // Unique constraint: A track can only be distributed to a DSP once
            builder.HasIndex(td => new { td.TrackId, td.DspId })
                .IsUnique()
                .HasDatabaseName("IX_TrackDistributions_TrackId_DspId");

            // Relationships
            builder.HasOne(td => td.Track)
                .WithMany(t => t.Distributions)
                .HasForeignKey(td => td.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(td => td.Dsp)
                .WithMany(d => d.TrackDistributions)
                .HasForeignKey(td => td.DspId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
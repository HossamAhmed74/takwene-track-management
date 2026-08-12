using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class DspConfiguration : IEntityTypeConfiguration<Dsp>
    {
        public void Configure(EntityTypeBuilder<Dsp> builder)
        {
            // Table Name
            builder.ToTable("Dsps");

            // Primary Key
            builder.HasKey(d => d.Id);

            // Properties
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Indexes
            builder.HasIndex(d => d.Name)
                .IsUnique()
                .HasDatabaseName("IX_Dsps_Name");

            // Relationships
            builder.HasMany(d => d.TrackDistributions)
                .WithOne(td => td.Dsp)
                .HasForeignKey(td => td.DspId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceManagement.Dal.Configurations;

public class ConferenceConfiguration : IEntityTypeConfiguration<Conference>
{
    public void Configure(EntityTypeBuilder<Conference> builder)
    {
        builder.HasKey(conference => conference.ConferenceId);

        builder.Property(conference => conference.ConferenceId)
            .ValueGeneratedOnAdd();

        builder.Property(conference => conference.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(conference => conference.Description)
            .HasMaxLength(1000);

        builder.Property(conference => conference.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(conference => conference.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(conference => conference.Status)
            .IsRequired()
            .HasMaxLength(50);
    }
}
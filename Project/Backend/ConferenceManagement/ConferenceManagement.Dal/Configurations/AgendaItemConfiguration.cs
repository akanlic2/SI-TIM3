using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceManagement.Dal.Configurations;

public class AgendaItemConfiguration : IEntityTypeConfiguration<AgendaItem>
{
    public void Configure(EntityTypeBuilder<AgendaItem> builder)
    {
        builder.HasKey(a => a.AgendaItemId);

        builder.Property(a => a.AgendaItemId)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Relacija prema Conference (Cascade delete)
        builder.HasOne(a => a.Conference)
            .WithMany(c => c.AgendaItems)
            .HasForeignKey(a => a.ConferenceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacija prema Session (nullable, bez cascade — session može biti obrisan zasebno)
        builder.HasOne(a => a.Session)
            .WithOne(s => s.AgendaItem)
            .HasForeignKey<AgendaItem>(a => a.SessionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Relacija prema Room (nullable)
        builder.HasOne(a => a.Room)
            .WithMany()
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}

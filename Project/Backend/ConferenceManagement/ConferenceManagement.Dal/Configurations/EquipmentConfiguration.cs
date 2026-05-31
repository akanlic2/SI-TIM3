using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceManagement.Dal.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.EquipmentId);

        builder.Property(e => e.EquipmentId)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Quantity)
            .IsRequired();

        builder.Property(e => e.AvailableQuantity)
            .IsRequired();

        builder.Property(e => e.IsAvailable)
            .IsRequired();

        builder.Property(e => e.AvailabilityStatus)
            .HasMaxLength(50);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // SessionId je nullable — null znači globalni inventar
        builder.HasOne(e => e.Session)
            .WithMany(s => s.Equipments)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}

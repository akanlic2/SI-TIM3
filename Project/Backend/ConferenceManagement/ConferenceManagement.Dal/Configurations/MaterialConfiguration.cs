using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceManagement.Dal.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.HasKey(m => m.MaterialId);

        builder.Property(m => m.MaterialId)
            .ValueGeneratedOnAdd();

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.FileUrl)
            .IsRequired();

        builder.Property(m => m.MaterialType)
            .HasMaxLength(100);

        // Kaskadno brisanje — kada se obriše sesija, brišu se i njeni materijali
        builder.HasOne(m => m.Session)
            .WithMany(s => s.Materials)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
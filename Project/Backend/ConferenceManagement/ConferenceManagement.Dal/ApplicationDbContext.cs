using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Conference> Conferences { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<ConferenceRegistration> ConferenceRegistrations { get; set; }
    public DbSet<SessionRegistration> SessionRegistrations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<LogisticsTask> LogisticsTasks { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AgendaItem> AgendaItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class SessionRegistrationRepository : ISessionRegistrationRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRegistrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SessionRegistration registration) =>
        await _context.SessionRegistrations.AddAsync(registration);

    public async Task<SessionRegistration?> GetBySessionAndUserAsync(Guid sessionId, Guid userId) =>
        await _context.SessionRegistrations
            .FirstOrDefaultAsync(r => r.SessionId == sessionId && r.UserId == userId);

    public async Task<SessionRegistration?> GetByIdAsync(Guid registrationId) =>
        await _context.SessionRegistrations
            .FirstOrDefaultAsync(r => r.SessionRegistrationId == registrationId);

    public async Task<List<SessionRegistration>> GetConfirmedRegistrationsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _context.SessionRegistrations
            .Include(r => r.Session)
            .ThenInclude(s => s.Room)
            .Include(r => r.Session)
            .ThenInclude(s => s.SessionRegistrations)
            .ThenInclude(sr => sr.User)
            .Where(r => r.UserId == userId && r.RegistrationStatus == "Confirmed")
            .ToListAsync(cancellationToken);

    public async Task DeleteAsync(SessionRegistration registration)
    {
        _context.SessionRegistrations.Remove(registration);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(SessionRegistration registration)
    {
        _context.SessionRegistrations.Update(registration);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
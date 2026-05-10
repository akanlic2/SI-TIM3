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

    public async Task UpdateAsync(SessionRegistration registration)
    {
        _context.SessionRegistrations.Update(registration);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
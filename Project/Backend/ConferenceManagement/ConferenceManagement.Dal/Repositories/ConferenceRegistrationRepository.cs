using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class ConferenceRegistrationRepository : IConferenceRegistrationRepository
{
    private readonly ApplicationDbContext _context;

    public ConferenceRegistrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ConferenceRegistration registration, CancellationToken cancellationToken = default) =>
        await _context.ConferenceRegistrations.AddAsync(registration, cancellationToken);

    public async Task<ConferenceRegistration?> GetByConferenceAndUserAsync(
        Guid conferenceId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _context.ConferenceRegistrations
            .FirstOrDefaultAsync(r => r.ConferenceId == conferenceId && r.UserId == userId, cancellationToken);

    public async Task<ConferenceRegistration?> GetByIdAsync(Guid registrationId, CancellationToken cancellationToken = default) =>
        await _context.ConferenceRegistrations
            .FirstOrDefaultAsync(r => r.ConferenceRegistrationId == registrationId, cancellationToken);

    public async Task<List<ConferenceRegistration>> GetConfirmedRegistrationsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _context.ConferenceRegistrations
            .AsNoTracking()
            .Include(r => r.Conference)
            .Where(r => r.UserId == userId && r.RegistrationStatus == "Confirmed")
            .ToListAsync(cancellationToken);

    public async Task<List<ConferenceRegistration>> GetRegistrationsByConferenceAsync(
        Guid conferenceId,
        CancellationToken cancellationToken = default) =>
        await _context.ConferenceRegistrations
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.ConferenceId == conferenceId)
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync(cancellationToken);

    public async Task<int> GetConfirmedCountForConferenceAsync(Guid conferenceId, CancellationToken cancellationToken = default) =>
        await _context.ConferenceRegistrations
            .CountAsync(r => r.ConferenceId == conferenceId && r.RegistrationStatus == "Confirmed", cancellationToken);

    public async Task UpdateAsync(ConferenceRegistration registration, CancellationToken cancellationToken = default)
    {
        _context.ConferenceRegistrations.Update(registration);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}

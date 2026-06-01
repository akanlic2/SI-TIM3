using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Equipment>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Equipments.ToListAsync(cancellationToken);

    public async Task<Equipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Equipments.FindAsync(new object[] { id }, cancellationToken);

    public async Task<List<Equipment>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        await _context.Equipments
            .Where(e => e.SessionId == sessionId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default) =>
        await _context.Equipments.AddAsync(equipment, cancellationToken);

    public async Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        _context.Equipments.Update(equipment);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        _context.Equipments.Remove(equipment);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}

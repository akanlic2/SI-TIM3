using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly ApplicationDbContext _context;
    public MaterialRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Material material, CancellationToken ct) => await _context.Materials.AddAsync(material, ct);
    public async Task<List<Material>> GetBySessionIdAsync(Guid sid, CancellationToken ct) =>
        await _context.Materials.Where(m => m.SessionId == sid).ToListAsync(ct);
    public async Task SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);
}
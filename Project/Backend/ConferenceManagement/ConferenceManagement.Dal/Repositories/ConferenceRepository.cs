using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class ConferenceRepository : GenericRepository<Conference>, IConferenceRepository
{
    public ConferenceRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    // Dodaj ovo:
    public new async Task<Conference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Conference>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(c => c.Category == category).ToListAsync(cancellationToken);
    }
}
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class ConferenceRepository : GenericRepository<Conference>, IConferenceRepository
{
    public ConferenceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Conference>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Category == category)
            .ToListAsync(cancellationToken);
    }
}
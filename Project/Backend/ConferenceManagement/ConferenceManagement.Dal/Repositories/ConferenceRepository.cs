using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class ConferenceRepository : GenericRepository<Conference>, IConferenceRepository
{
    public ConferenceRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public new async Task<Conference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Conference>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Category == category)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Conference> Items, int TotalCount)> GetPagedFilteredAsync(
        int page,
        int pageSize,
        string? search,
        string? location,
        string? category,
        string? status,
        bool includeInactiveAndDraft,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 6;

        var query = _dbSet
            .AsNoTracking()
            .AsQueryable();

        if (!includeInactiveAndDraft)
        {
            query = query.Where(c =>
                c.Status.ToLower() == "active");
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();

            query = query.Where(c =>
                c.Title.ToLower().Contains(normalizedSearch) ||
                c.Description.ToLower().Contains(normalizedSearch));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var normalizedLocation = location.Trim().ToLower();

            query = query.Where(c =>
                c.Location.ToLower().Contains(normalizedLocation));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim().ToLower();

            query = query.Where(c =>
                c.Category.ToLower() == normalizedCategory);
        }

        if (!string.IsNullOrWhiteSpace(status) && includeInactiveAndDraft)
        {
            var normalizedStatus = status.Trim().ToLower();

            query = query.Where(c =>
                c.Status.ToLower() == normalizedStatus);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
    public async Task<Conference?> GetByIdWithOrganizersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Organizers)
            .FirstOrDefaultAsync(c => c.ConferenceId == id, cancellationToken);
    }
}
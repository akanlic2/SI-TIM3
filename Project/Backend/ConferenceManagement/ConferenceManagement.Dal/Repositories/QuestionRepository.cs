using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public QuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByIdAsync(Guid id) =>
        await _context.Questions.FindAsync(id);

    public async Task AddAsync(Question question) =>
        await _context.Questions.AddAsync(question);

    public async Task<IEnumerable<Question>> GetBySessionIdAsync(Guid sessionId)
    {
        return await _context.Questions
            .Where(q => q.SessionId == sessionId)
            .Include(q => q.User)
            .OrderBy(q => q.AskedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
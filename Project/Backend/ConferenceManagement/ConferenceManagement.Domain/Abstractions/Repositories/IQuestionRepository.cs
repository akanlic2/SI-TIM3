using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id);
    Task AddAsync(Question question);
    Task<IEnumerable<Question>> GetBySessionIdAsync(Guid sessionId);
    Task SaveChangesAsync();
}

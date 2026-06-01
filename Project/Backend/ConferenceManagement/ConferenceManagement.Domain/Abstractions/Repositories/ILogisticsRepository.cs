using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories
{
    public interface ILogisticsRepository
    {
        Task<IEnumerable<LogisticsTask>> GetByConferenceIdAsync(Guid conferenceId, string? taskType);
        Task<bool> IsUserOrganizerOfConferenceAsync(Guid conferenceId, Guid userId);
        Task AddAsync(LogisticsTask task);
        Task<LogisticsTask?> GetByIdAsync(Guid logisticsTaskId);
        Task UpdateAsync(LogisticsTask task);
        Task DeleteAsync(LogisticsTask task);
    }
}
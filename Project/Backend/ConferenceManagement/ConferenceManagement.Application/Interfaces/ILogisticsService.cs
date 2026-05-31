using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenceManagement.Application.DTOs.Logistics;

namespace ConferenceManagement.Application.Interfaces
{
    public interface ILogisticsService
    {
        Task<IEnumerable<LogisticsTaskDto>> GetLogisticsForConferenceAsync(Guid conferenceId, string? taskType, Guid currentUserId, string currentUserRole);
        Task<LogisticsTaskDto> CreateLogisticsTaskAsync(Guid conferenceId, CreateLogisticsTaskDto dto, Guid currentUserId, string currentUserRole);
        Task<LogisticsTaskDto> UpdateLogisticsTaskAsync(Guid logisticsTaskId, UpdateLogisticsTaskDto dto, Guid currentUserId, string currentUserRole);
        Task DeleteLogisticsTaskAsync(Guid logisticsTaskId, Guid currentUserId, string currentUserRole);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenceManagement.Application.DTOs.Logistics;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services
{
    public class LogisticsService : ILogisticsService
    {
        private readonly ILogisticsRepository _logisticsRepository;

        public LogisticsService(ILogisticsRepository logisticsRepository)
        {
            _logisticsRepository = logisticsRepository;
        }

        public async Task<IEnumerable<LogisticsTaskDto>> GetLogisticsForConferenceAsync(Guid conferenceId, string? taskType, Guid currentUserId, string currentUserRole)
        {
            // Prilagođeno tvojim rolama: Ako klijent nije "admin-sistema", provjeravamo da li je baš organizator TE konferencije
            if (currentUserRole != "admin-sistema")
            {
                var isOrganizer = await _logisticsRepository.IsUserOrganizerOfConferenceAsync(conferenceId, currentUserId);
                if (!isOrganizer)
                {
                    // Ovo će tvoj globalni Exception Handler presresti i vratiti čist HTTP 403 JSON klijentu
                    throw new UnauthorizedAccessException("Nemate permisije za pregled logističkih aktivnosti ove konferencije jer niste njen organizator.");
                }
            }

            var tasks = await _logisticsRepository.GetByConferenceIdAsync(conferenceId, taskType);

            return tasks.Select(t => new LogisticsTaskDto
            {
                LogisticsTaskId = t.LogisticsTaskId,
                ConferenceId = t.ConferenceId,
                Title = t.Title,
                Description = t.Description,
                TaskType = t.TaskType,
                DueDate = t.DueDate,
                Status = t.Status
            });
        }
        public async Task<LogisticsTaskDto> CreateLogisticsTaskAsync(Guid conferenceId, CreateLogisticsTaskDto dto, Guid currentUserId, string currentUserRole)
        {
            // Sigurnosna provjera (S46 - BE Role Guard)
            if (currentUserRole != "admin-sistema")
            {
                var isOrganizer = await _logisticsRepository.IsUserOrganizerOfConferenceAsync(conferenceId, currentUserId);
                if (!isOrganizer)
                {
                    throw new UnauthorizedAccessException("Nemate permisije za dodavanje aktivnosti ovoj konferenciji jer niste njen organizator.");
                }
            }

            // Kreiranje domenskog entiteta na osnovu DTO-a i parametara
            var newTask = new LogisticsTask
            {
                LogisticsTaskId = Guid.NewGuid(),
                ConferenceId = conferenceId,
                Title = dto.Title,
                Description = dto.Description,
                TaskType = dto.TaskType,
                DueDate = dto.DueDate,
                Status = dto.Status
                // Napomena: EF će sam povezati 'Conference' navigacijski objekat preko 'ConferenceId' pri spasavanju
            };

            await _logisticsRepository.AddAsync(newTask);

            // Vraćamo nazad puni model sa generisanim ID-jem
            return new LogisticsTaskDto
            {
                LogisticsTaskId = newTask.LogisticsTaskId,
                ConferenceId = newTask.ConferenceId,
                Title = newTask.Title,
                Description = newTask.Description,
                TaskType = newTask.TaskType,
                DueDate = newTask.DueDate,
                Status = newTask.Status
            };
        }
        public async Task<LogisticsTaskDto> UpdateLogisticsTaskAsync(Guid logisticsTaskId, UpdateLogisticsTaskDto dto, Guid currentUserId, string currentUserRole)
        {
            var existingTask = await _logisticsRepository.GetByIdAsync(logisticsTaskId);
            if (existingTask == null)
            {
                throw new KeyNotFoundException("Logistička aktivnost ne postoji.");
            }

            if (currentUserRole != "admin-sistema")
            {
                var isOrganizer = await _logisticsRepository.IsUserOrganizerOfConferenceAsync(existingTask.ConferenceId, currentUserId);
                if (!isOrganizer)
                {
                    throw new UnauthorizedAccessException("Nemate permisije za izmjenu aktivnosti na ovoj konferenciji.");
                }
            }

            // Mapiranje izmjena
            existingTask.Title = dto.Title;
            existingTask.Description = dto.Description;
            existingTask.TaskType = dto.TaskType;
            existingTask.DueDate = dto.DueDate;
            existingTask.Status = dto.Status;

            // Ovo će sada automatski baciti InvalidOperationException ako se desi konflikt
            await _logisticsRepository.UpdateAsync(existingTask);

            return new LogisticsTaskDto
            {
                LogisticsTaskId = existingTask.LogisticsTaskId,
                ConferenceId = existingTask.ConferenceId,
                Title = existingTask.Title,
                Description = existingTask.Description,
                TaskType = existingTask.TaskType,
                DueDate = existingTask.DueDate,
                Status = existingTask.Status
            };
        }

        public async Task DeleteLogisticsTaskAsync(Guid logisticsTaskId, Guid currentUserId, string currentUserRole)
        {
            var existingTask = await _logisticsRepository.GetByIdAsync(logisticsTaskId);
            if (existingTask == null)
            {
                throw new KeyNotFoundException("Logistička aktivnost ne postoji i ne može biti obrisana.");
            }

            if (currentUserRole != "admin-sistema")
            {
                var isOrganizer = await _logisticsRepository.IsUserOrganizerOfConferenceAsync(existingTask.ConferenceId, currentUserId);
                if (!isOrganizer)
                {
                    throw new UnauthorizedAccessException("Nemate permisije za brisanje logističkih aktivnosti sa ove konferencije.");
                }
            }

            await _logisticsRepository.DeleteAsync(existingTask);
        }
    }
}
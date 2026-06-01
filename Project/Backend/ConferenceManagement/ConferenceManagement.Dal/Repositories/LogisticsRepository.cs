using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories
{
    public class LogisticsRepository : ILogisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public LogisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LogisticsTask>> GetByConferenceIdAsync(Guid conferenceId, string? taskType)
        {
            // Budući da entitet zahtijeva 'Conference' objekat, radimo .Include da se napuni relacija
            var query = _context.LogisticsTasks
                .Include(t => t.Conference)
                .Where(t => t.ConferenceId == conferenceId);

            if (!string.IsNullOrWhiteSpace(taskType))
            {
                var lowerType = taskType.ToLower();
                query = query.Where(t => t.TaskType.ToLower() == lowerType);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> IsUserOrganizerOfConferenceAsync(Guid conferenceId, Guid userId)
        {
            return await _context.Conferences
                .Where(c => c.ConferenceId == conferenceId)
                .AnyAsync(c => c.Organizers.Any(u => u.UserId == userId));
        }
        public async Task AddAsync(LogisticsTask task)
        {
            await _context.LogisticsTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }
        public async Task<LogisticsTask?> GetByIdAsync(Guid logisticsTaskId)
        {
            // Koristimo AsNoTracking() kod provjere trenutnog stanja u servisu 
            // kako nam se EF tracking ne bi sukobio sa kasnijim update-om.
            return await _context.LogisticsTasks
                .Include(t => t.Conference)
                .FirstOrDefaultAsync(t => t.LogisticsTaskId == logisticsTaskId);
        }

        public async Task UpdateAsync(LogisticsTask task)
        {
            try
            {
                _context.LogisticsTasks.Update(task);
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                // Ovdje hvatamo EF grešku i pretvaramo je u standardni C# exception
                throw new InvalidOperationException("Konflikt pri istovremenom uređivanju! Podaci su u međuvremenu izmijenjeni.");
            }
        }

        public async Task DeleteAsync(LogisticsTask task)
        {
            _context.LogisticsTasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
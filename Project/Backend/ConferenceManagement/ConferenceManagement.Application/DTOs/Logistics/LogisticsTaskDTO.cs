using System;

namespace ConferenceManagement.Application.DTOs.Logistics
{
    public class LogisticsTaskDto
    {
        public Guid LogisticsTaskId { get; set; }
        public Guid ConferenceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
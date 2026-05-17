using System.ComponentModel.DataAnnotations;

namespace ConferenceManagement.Application.DTOs.Agenda;

public class UpdateAgendaItemDto
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public Guid? SessionId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid? RoomId { get; set; }
}

namespace ConferenceManagement.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateSessionDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public Guid ConferenceId { get; set; }

    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public string SessionType { get; set; } = string.Empty;
}
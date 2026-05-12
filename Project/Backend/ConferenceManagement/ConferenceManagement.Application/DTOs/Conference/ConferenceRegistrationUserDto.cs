using ConferenceManagement.Application.DTOs.User;

namespace ConferenceManagement.Application.DTOs.Conference;

public class ConferenceRegistrationUserDto
{
    public Guid ConferenceRegistrationId { get; set; }
    public Guid ConferenceId { get; set; }
    public Guid UserId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string RegistrationStatus { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

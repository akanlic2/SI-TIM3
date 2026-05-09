namespace ConferenceManagement.Domain.Entities;

public class Question
{
    public Guid QuestionId { get; set; }
    public Guid UserId { get; set; }
    public Guid SessionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime AskedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;

    public User User { get; set; }
    public Session Session { get; set; }
}
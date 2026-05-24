namespace ConferenceManagement.Application.DTOs.Question;

public class QuestionDto
{
    public Guid QuestionId { get; set; }
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime AskedAt { get; set; }
    public string? Answer { get; set; }
    public string Status { get; set; } = string.Empty;
}

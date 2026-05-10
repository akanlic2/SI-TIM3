namespace ConferenceManagement.Application.DTOs.Conference;

public class ConferenceQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 6;

    public string? Search { get; set; }
    public string? Location { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
}

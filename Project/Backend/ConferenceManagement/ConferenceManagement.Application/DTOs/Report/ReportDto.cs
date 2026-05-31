namespace ConferenceManagement.Application.DTOs.Report;

public record ConferenceReportDto
{
    public Guid ConferenceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public RegistrationStatsDto RegistrationStats { get; set; } = new();
    public List<SessionReportDto> Sessions { get; set; } = new();
    public int TotalMaterials { get; set; }
    public int TotalSpeakers { get; set; }
}

public record RegistrationStatsDto
{
    public int Total { get; set; }
    public int Confirmed { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
}

public record SessionReportDto
{
    public Guid SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int RegisteredCount { get; set; }
    public int RoomCapacity { get; set; }
    public int SpeakerCount { get; set; }
    public int MaterialCount { get; set; }
}
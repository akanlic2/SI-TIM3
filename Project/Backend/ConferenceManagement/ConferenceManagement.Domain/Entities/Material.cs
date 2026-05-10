namespace ConferenceManagement.Domain.Entities;

public class Material
{
    public Guid MaterialId { get; set; }
    public Guid? ConferenceId { get; set; }
    public Guid? SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public Conference Conference { get; set; }
    public Session Session { get; set; }
}
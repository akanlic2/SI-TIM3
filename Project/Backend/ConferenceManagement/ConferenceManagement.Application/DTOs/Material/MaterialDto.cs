namespace ConferenceManagement.Application.DTOs.Material;

public class MaterialDto
{
    public Guid MaterialId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
}
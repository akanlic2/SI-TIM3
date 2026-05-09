using System.ComponentModel.DataAnnotations;

namespace ConferenceManagement.Application.DTOs.Conference;

public record CreateConferenceDto
{
    [Required]
    [MinLength(3, ErrorMessage = "Naslov mora imati najmanje 3 karaktera.")]
    [MaxLength(100, ErrorMessage = "Naslov ne smije biti duži od 100 karaktera.")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(10, ErrorMessage = "Opis mora imati najmanje 10 karaktera.")]
    [MaxLength(500, ErrorMessage = "Opis ne smije biti duži od 500 karaktera.")]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Lokacija ne smije biti prazna.")]
    public string Location { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Maksimalan broj učesnika mora biti veći od 0.")]
    public int MaxParticipants { get; set; }
}
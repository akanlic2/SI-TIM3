using System.ComponentModel.DataAnnotations;

namespace ConferenceManagement.Application.DTOs.Equipment;

public class CreateEquipmentDto
{
    [Required(ErrorMessage = "Naziv opreme je obavezan.")]
    [MinLength(1, ErrorMessage = "Naziv ne može biti prazan.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tip opreme je obavezan.")]
    [MinLength(1, ErrorMessage = "Tip ne može biti prazan.")]
    public string Type { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti veća od 0.")]
    public int Quantity { get; set; }
}

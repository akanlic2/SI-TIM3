using System.ComponentModel.DataAnnotations;

namespace ConferenceManagement.Application.DTOs.Equipment;

public class AssignEquipmentDto
{
    [Required(ErrorMessage = "ID opreme je obavezan.")]
    public Guid EquipmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti veća od 0.")]
    public int Quantity { get; set; }
}

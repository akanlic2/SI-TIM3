using System;
using System.ComponentModel.DataAnnotations;

namespace ConferenceManagement.Application.DTOs.Logistics
{
    public class UpdateLogisticsTaskDto
    {
        [Required(ErrorMessage = "Naslov je obavezan.")]
        [StringLength(150, ErrorMessage = "Naslov ne može biti duži od 150 karaktera.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis je obavezan.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tip aktivnosti je obavezan.")]
        public string TaskType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rok izvršenja je obavezan.")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Status je obavezan.")]
        public string Status { get; set; } = string.Empty;
    }
}
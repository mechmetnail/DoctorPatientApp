using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.Models
{
    public class PatientDto
    {
        [Required, MaxLength(50)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(13)]
        public string Efka { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? City { get; set; }

        public string? Phone { get; set; }

        [StringLength(14)]
        public string? AFM { get; set; }

        public IFormFile? Photo { get; set; }

        [Required]
        public DateTime EnrollingDate { get; set; } = DateTime.Now;
    }
}

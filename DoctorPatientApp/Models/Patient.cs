using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.Models
{
    public class Patient
    {
      
        public int PatientId { get; set; }

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
        
        public string? PhotoPath { get; set; }

        [Required]
        public DateTime EnrollingDate { get; set; } = DateTime.Now;

        public ICollection<Examination> Examination { get; set; }

    }
}

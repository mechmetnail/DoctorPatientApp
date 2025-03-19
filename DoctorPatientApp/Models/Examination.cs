using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.Models
{
    public class Examination
    {
        
        public int ExaminationId { get; set; }

        [Required]
        public DateTime ExamDate { get; set; } = DateTime.Now;
                
        public string? Results { get; set; }

        public string? Notes { get; set; }

        public string? Medicines { get; set; }

        public string? ProblemPhotoPath { get; set; }

        public string? ProblemPhotoPath2 { get; set; }

        public string? ProblemPhotoPath3 { get; set; }

        public string? ProblemPhotoPath4 { get; set; }

        public int PatientId { get; set; }

        public Patient Patient { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.Models
{
    public class ExaminationDto
    {
        
        public int ExaminationId { get; set; }

        [Required]
        public DateTime ExamDate { get; set; } = DateTime.Now;

        public string? Results { get; set; }

        public string? Notes { get; set; }

        public string? Medicines { get; set; }


        [Required(ErrorMessage = "Hasta seçimi zorunludur!")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçersiz hasta ID!")]
        public int PatientId { get; set; }

        public string? ProblemPhotoPath { get; set; }
        public string? ProblemPhotoPath2 { get; set; }
        public string? ProblemPhotoPath3 { get; set; }
        public string? ProblemPhotoPath4 { get; set; }

        public IFormFile? ProblemPhoto { get; set; }

        public IFormFile? ProblemPhoto2 { get; set; }

        public IFormFile? ProblemPhoto3 { get; set; }

        public IFormFile? ProblemPhoto4 { get; set; }
    }
}

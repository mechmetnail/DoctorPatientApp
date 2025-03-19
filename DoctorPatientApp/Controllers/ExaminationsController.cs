using DoctorPatientApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.Controllers
{
    public class ExaminationsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        public ExaminationsController(DataContext context, IWebHostEnvironment env) 
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var examinations = _context.Examinations
        .Include(e => e.Patient) // İlişkili Patient'ı yükle
        .OrderByDescending(p => p.PatientId)
        .ToList();

            return View(examinations);            
        }
    }
}

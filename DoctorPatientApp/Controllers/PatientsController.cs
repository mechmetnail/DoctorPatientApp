using DoctorPatientApp.Data;
using DoctorPatientApp.Models;
using DoctorPatientApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DoctorPatientApp.Controllers
{
    //[Authorize]
    public class PatientsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public PatientsController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var patients = _context.Patients.OrderByDescending(p => p.PatientId).ToList();
            return View(patients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientDto patientDto)
        {
            if (patientDto.Photo == null)
            {
                ModelState.AddModelError("Photo", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(patientDto);
            }

            // Fotoğrafı sunucuya kaydet
            string photoFileName = await SavePhotoAsync(patientDto.Photo!);

            // DTO'dan Patient'a dönüşüm
            var patient = new Patient
            {
                FullName = patientDto.FullName,
                Efka = patientDto.Efka,
                Address = patientDto.Address,
                City = patientDto.City,
                Phone = patientDto.Phone,
                AFM = patientDto.AFM,
                PhotoPath = "/img/" + photoFileName, // Yolu doğru ayarla
                EnrollingDate = patientDto.EnrollingDate
            };

            // Veritabanına ekle
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Patients");
        }

        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "img");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }
            return uniqueFileName;
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return RedirectToAction("Index", "Patients");
            }

            // Patient'dan PatientDto'ya dönüşüm
            var patientDto = new PatientDto
            {
                FullName = patient.FullName,
                Efka = patient.Efka,
                Address = patient.Address,
                City = patient.City,
                Phone = patient.Phone,
                AFM = patient.AFM,
                EnrollingDate = patient.EnrollingDate
            };

            ViewData["PatientId"] = patient.PatientId;
            ViewData["PhotoPath"] = patient.PhotoPath; // Mevcut fotoğrafı View'a gönder
            return View(patientDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientDto patientDto)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return RedirectToAction("Index", "Patients");
            }

            if (!ModelState.IsValid)
            {
                ViewData["PhotoPath"] = _context.Patients.AsNoTracking().FirstOrDefault(p => p.PatientId == id)?.PhotoPath;
                return View(patientDto);
            }

            // Yeni fotoğraf yüklendi mi?
            if (patientDto.Photo != null)
            {
                // Eski fotoğrafı sil
                if (!string.IsNullOrEmpty(patient.PhotoPath))
                {
                    var oldPhotoPath = Path.Combine(_env.WebRootPath, patient.PhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }

                // Yeni fotoğrafı kaydet
                var newPhotoFileName = await SavePhotoAsync(patientDto.Photo);
                patient.PhotoPath = "/img/" + newPhotoFileName;
            }

            patient.FullName = patientDto.FullName;
            patient.Efka = patientDto.Efka;
            patient.Address = patientDto.Address;
            patient.City = patientDto.City;
            patient.Phone = patientDto.Phone;
            patient.AFM = patientDto.AFM;
            patient.EnrollingDate = patientDto.EnrollingDate;

            try
            {
                _context.Update(patient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(patient.PatientId))
                {
                    return RedirectToAction("Index", "Patients");
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }


        [HttpGet]
        public IActionResult DeleteConfirmation(int id)
        {
            var patient = _context.Patients.Find(id);

            if (patient == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var patientDto = new PatientDto
            {
                FullName = patient.FullName,
                Efka = patient.Efka,
                Address = patient.Address,
                City = patient.City,
                Phone = patient.Phone,
                AFM = patient.AFM,
                EnrollingDate = patient.EnrollingDate
            };

            ViewData["PatientId"] = patient.PatientId;
            ViewData["PhotoPath"] = patient.PhotoPath;
            return View(patientDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {

            var patient = _context.Patients.Find(id);
            if (patient == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(patient.PhotoPath))
            {
                string cleanPhotoPath = patient.PhotoPath
                    .TrimStart('/', '\\')
                    .Replace("/", "\\");

                string fullPath = Path.Combine(
                    _env.WebRootPath,
                    "img",
                    cleanPhotoPath
                );

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            try
            {
                _context.Patients.Remove(patient);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"{patient.FullName} silindi.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Silme işlemi başarısız oldu.";
            }

            return RedirectToAction(nameof(Index));

        }


        public IActionResult Details(int id)
        {
            var patient = _context.Patients
                .Include(p => p.Examination)
                .FirstOrDefault(p => p.PatientId == id);

            if (patient == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = patient.PatientId;
            return View(patient);
        }

        [HttpGet]
        public async Task<IActionResult> CreateExam(int id) 
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new ExamCreateViewModel
            {
                ExaminationDto = new ExaminationDto
                {
                    PatientId = id
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam(ExamCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == viewModel.ExaminationDto.PatientId);

            if (!patientExists)
            {
                ModelState.AddModelError("ExaminationDto.PatientId", "Geçersiz hasta seçimi!");               
                return View(viewModel);
            }

            string? problemPhotoPath = null;
            if (viewModel.ExaminationDto.ProblemPhoto != null)
            {
                problemPhotoPath = await SaveProblemPhotoAsync(viewModel.ExaminationDto.ProblemPhoto);
            }

            string? problemPhotoPath2 = null;
            if (viewModel.ExaminationDto.ProblemPhoto2 != null)
            {
                problemPhotoPath2 = await SaveProblemPhotoAsync(viewModel.ExaminationDto.ProblemPhoto2);
            }

            string? problemPhotoPath3 = null;
            if (viewModel.ExaminationDto.ProblemPhoto3 != null)
            {
                problemPhotoPath3 = await SaveProblemPhotoAsync(viewModel.ExaminationDto.ProblemPhoto3);
            }

            string? problemPhotoPath4 = null;
            if (viewModel.ExaminationDto.ProblemPhoto4 != null)
            {
                problemPhotoPath4 = await SaveProblemPhotoAsync(viewModel.ExaminationDto.ProblemPhoto4);
            }
            var examination = new Examination
            {
                ExamDate = viewModel.ExaminationDto.ExamDate,
                Results = viewModel.ExaminationDto.Results,
                Notes = viewModel.ExaminationDto.Notes,
                Medicines = viewModel.ExaminationDto.Medicines,
                ProblemPhotoPath = problemPhotoPath,
                ProblemPhotoPath2 = problemPhotoPath2,
                ProblemPhotoPath3 = problemPhotoPath3,
                ProblemPhotoPath4 = problemPhotoPath4,
                PatientId = viewModel.ExaminationDto.PatientId
            };

            _context.Examinations.Add(examination);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Patients", new { id = viewModel.ExaminationDto.PatientId });
        }


        private async Task<string> SaveProblemPhotoAsync(IFormFile photo)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "img");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + "_" + photo.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return "/img/" + uniqueFileName;
        }

        [HttpGet]
        public async Task<IActionResult> EditExam(int id)
        {
            var examination = await _context.Examinations.FindAsync(id);
            if (examination == null)
            {
                return NotFound();
            }

            var viewModel = new ExamCreateViewModel
            {
                ExaminationDto = new ExaminationDto
                {
                    ExaminationId = examination.ExaminationId,
                    PatientId = examination.PatientId,
                    ExamDate = examination.ExamDate,
                    Results = examination.Results,
                    Notes = examination.Notes,
                    Medicines = examination.Medicines,
                    ProblemPhotoPath = examination.ProblemPhotoPath,
                    ProblemPhotoPath2 = examination.ProblemPhotoPath2,
                    ProblemPhotoPath3 = examination.ProblemPhotoPath3,
                    ProblemPhotoPath4 = examination.ProblemPhotoPath4
                }
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExam(int id, ExamCreateViewModel viewModel)
        {
            if (id != viewModel.ExaminationDto.ExaminationId)
            {
                return NotFound();
            }

            var existingExamination = await _context.Examinations.FindAsync(id);
            if (existingExamination == null)
            {
                return NotFound();
            }

            // Temel alanları güncelle
            existingExamination.ExamDate = viewModel.ExaminationDto.ExamDate;
            existingExamination.Results = viewModel.ExaminationDto.Results;
            existingExamination.Notes = viewModel.ExaminationDto.Notes;
            existingExamination.Medicines = viewModel.ExaminationDto.Medicines;

            // Fotoğraf güncellemeleri
            existingExamination.ProblemPhotoPath = await HandlePhotoUpdate(
                viewModel.ExaminationDto.ProblemPhoto,
                existingExamination.ProblemPhotoPath
            );

            existingExamination.ProblemPhotoPath2 = await HandlePhotoUpdate(
                viewModel.ExaminationDto.ProblemPhoto2,
                existingExamination.ProblemPhotoPath2
            );

            // Diğer fotoğraflar için aynı işlem

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Patients", new { id = existingExamination.PatientId });
        }

        private async Task<string?> HandlePhotoUpdate(IFormFile? newPhoto, string? currentPath)
        {
            if (newPhoto != null && newPhoto.Length > 0)
            {
                // Eski fotoğrafı sil
                if (!string.IsNullOrEmpty(currentPath))
                {
                    var fullPath = Path.Combine(_env.WebRootPath, currentPath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                // Yeni fotoğrafı kaydet
                return await SaveProblemPhotoAsync(newPhoto);
            }

            // Yeni fotoğraf yüklenmemişse mevcut yolu koru
            return currentPath;
        }



        private async Task UpdatePhotoAsync(IFormFile newPhoto, string existingPhotoPath, Action<string> setPathAction)
        {
            if (newPhoto != null)
            {
                // Eski fotoğrafı sil
                if (!string.IsNullOrEmpty(existingPhotoPath))
                {
                    DeleteProblemPhoto(existingPhotoPath);
                }
                // Yeni fotoğrafı kaydet
                var newPath = await SaveProblemPhotoAsync(newPhoto);
                setPathAction(newPath);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var examination = await _context.Examinations.FindAsync(id);
            if (examination == null)
            {
                return NotFound();
            }
            return View(examination);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteExam")]
        public async Task<IActionResult> DeleteExamConfirmed(int id)
        {
            var examination = await _context.Examinations.FindAsync(id);
            if (examination == null)
            {
                return NotFound();
            }

            // Fotoğrafları sil
            DeleteProblemPhoto(examination.ProblemPhotoPath);
            DeleteProblemPhoto(examination.ProblemPhotoPath2);
            DeleteProblemPhoto(examination.ProblemPhotoPath3);
            DeleteProblemPhoto(examination.ProblemPhotoPath4);

            _context.Examinations.Remove(examination);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Patients", new { id = examination.PatientId });
        }

        private void DeleteProblemPhoto(string photoPath)
        {
            if (!string.IsNullOrEmpty(photoPath))
            {
                var fullPath = Path.Combine(_env.WebRootPath, photoPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }


    }
}

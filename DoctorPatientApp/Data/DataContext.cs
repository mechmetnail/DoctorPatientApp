using DoctorPatientApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
            
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Examination>()
                .HasOne(e => e.Patient)
                .WithMany(p => p.Examination)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}

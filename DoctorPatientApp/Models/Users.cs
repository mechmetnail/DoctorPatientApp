using Microsoft.AspNetCore.Identity;

namespace DoctorPatientApp.Models
{
    public class Users : IdentityUser
    {
        public string AllName { get; set; }
    }
}

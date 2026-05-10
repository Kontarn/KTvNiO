using Microsoft.AspNetCore.Identity;

namespace KTvNiO.Models
{
    public enum UserRole
    {
        Student,
        Teacher,
        Developer,
        Support
    }

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

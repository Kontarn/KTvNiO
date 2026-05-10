using System.ComponentModel.DataAnnotations;
using KTvNiO.Models;

namespace KTvNiO.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Пароль должен содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public UserRole Role { get; set; } = UserRole.Student;
    }
}

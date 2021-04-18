using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models.Payloads
{
    public class PasswordResetPayload
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string NewPasswordConfirm { get; set; }
    }
}
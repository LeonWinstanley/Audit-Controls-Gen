using System.ComponentModel.DataAnnotations;
using FinalYearProject.Enums;

namespace FinalYearProject.Models.Payloads
{
    public class RegisterPayload
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public Role UserRole { get; set; }
    }
}
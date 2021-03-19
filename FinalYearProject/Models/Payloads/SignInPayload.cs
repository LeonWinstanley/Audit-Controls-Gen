using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models.Payloads
{
    public class SignInPayload
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
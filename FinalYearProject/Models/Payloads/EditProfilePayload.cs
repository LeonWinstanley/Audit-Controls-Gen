using FinalYearProject.Enums;

namespace FinalYearProject.Models.Payloads
{
    public class EditProfilePayload
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public Role Role { get; set; }
    }
}
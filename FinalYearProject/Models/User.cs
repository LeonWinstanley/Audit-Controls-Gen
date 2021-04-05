using System.Collections.Generic;
using FinalYearProject.Enums;
using Microsoft.AspNetCore.Identity;

namespace FinalYearProject.Models
{
    public class User : IdentityUser
    {
        public List<ControlEvaluations> ControlEvaluations { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
using System.Collections.Generic;
using FinalYearProject.Enums;

namespace FinalYearProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Role UserRole { get; set; }
        public List<ControlEvaluations> ControlEvaluations { get; set; }
    }
}
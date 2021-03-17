using System.Collections.Generic;

namespace FinalYearProject.Models
{
    public class ControlEvaluations
    {
        public int Id { get; set; }
        
        public List<ControlEvaluationControls> ControlsList { get; set; }
    }
}
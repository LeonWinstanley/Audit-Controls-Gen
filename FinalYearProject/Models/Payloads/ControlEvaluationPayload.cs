using System;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models.Payloads
{
    public class ControlEvaluationPayload
    {
        [Required]
        public string AuditTitle { get; set; }
        [Required]
        public string LeadAuditor { get; set; }
        
        public DateTimeOffset DateCreated { get; set; }
    }
}
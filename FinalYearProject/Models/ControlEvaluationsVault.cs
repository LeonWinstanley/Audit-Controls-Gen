using System;
using System.Collections.Generic;
using FinalYearProject.Enums;

namespace FinalYearProject.Models
{
    public class ControlEvaluationsVault
    {
        public int Id { get; set; }
        public string AuditName { get; set; }
        public string LeadAuditor { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string ReviewerEmail { get; set; }
        public List<ControlEvaluationControls> ControlsList { get; set; }
    }
}
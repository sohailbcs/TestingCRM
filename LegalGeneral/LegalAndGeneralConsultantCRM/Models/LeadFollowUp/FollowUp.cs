 using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.LeadFollowUp
{
    public class FollowUp
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int FollowUpId { get; set; }
		public string? EmployeeId { get; set; }
		public string? FollowUpType { get; set; }
		public LegalAndGeneralConsultantCRMUser Employee { get; set; }

		public int? LeadId { get; set; }
		public Lead Lead { get; set; }
		public DateTime? FollowUpDate { get; set; }
		public string? Status { get; set; }
		public DateTime? Reminder { get; set; }
		public string? Description { get; set; }
		public bool FollowUpCompleted { get; set; }



	}
}

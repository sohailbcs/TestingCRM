using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Leads
{
    public class LeadHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeadHistoryId { get; set; }

        // Foreign key to Lead
        public int? LeadId { get; set; }
        [ForeignKey("LeadId")]
        public Lead Lead { get; set; }  // Ensure you have 'using LegalAndGeneralConsultantCRM.Models.Leads;' for Lead class

        // Foreign key to User (assuming LegalAndGeneralConsultantCRMUser)
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public LegalAndGeneralConsultantCRMUser User { get; set; }

        public string? Status { get; set; }
        public string? Description { get; set; }
        public DateTime? LeadFollowupDate { get; set; }
    }
}

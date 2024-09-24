using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.ActivitiesLog
{
    public class ActivityLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityLogId { get; set; }
        public int? LeadId { get; set; }
        [ForeignKey("LeadId")]
        public Lead Lead { get; set; }   
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public LegalAndGeneralConsultantCRMUser User { get; set; }

        public string? Status { get; set; }
        public string? Action { get; set; }
        public DateTime? ActivityLogDate { get; set; }
    }
}

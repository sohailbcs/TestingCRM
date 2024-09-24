using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models
{
    public class LeadAssignEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllocationId { get; set; }

        [Required]
        public string EmployeeId { get; set; }
        public LegalAndGeneralConsultantCRMUser Employee { get; set; }

        [Required]
        public int LeadId { get; set; }
        public Lead Lead { get; set; }

        [Required]
        public DateTime AssignmentDate { get; set; }

     }
}

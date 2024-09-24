using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Students
{
    public class StudentEnrollment
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentEnrollmentId { get; set; }

        public int? LeadId { get; set; }
        public Lead Lead { get; set; }

        public int? StudentId { get; set; }
        public Student Student { get; set; }

        public string? UserId { get; set; }
        public LegalAndGeneralConsultantCRMUser User { get; set; }

        public string? EnrollmentStatus { get; set; } 

        public DateTime? EnrollmentDate { get; set; }
        public bool? IsAccepted { get; set; } = false;


    }
}

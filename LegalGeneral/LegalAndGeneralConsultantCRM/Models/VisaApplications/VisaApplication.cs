using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.Students;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.VisaApplications
{
    public class VisaApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }
        public int? StudentId { get; set; }
        public int? LeadId { get; set; }
        public string? UserId { get; set; }
        public Lead Lead { get; set; }
        public Student Student { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string? DestinationCountry { get; set; }
        public string? ApplicationNumber { get; set; }
        public string? VisaStatus { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}

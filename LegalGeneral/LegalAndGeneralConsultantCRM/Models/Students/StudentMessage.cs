using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Students
{
    public class StudentMessage
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentMessageId { get; set; }
        public int? StudentId { get; set; }
        public Student Student { get; set; }

        public int? LeadId { get; set; }
        public Lead Lead { get; set; }

        public string? Message { get; set; }
        public string?  Status { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}

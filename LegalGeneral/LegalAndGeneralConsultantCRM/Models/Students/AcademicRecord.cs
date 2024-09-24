using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Students
{
    public class AcademicRecord
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentAcademiaId { get; set; }
        public int? LeadId { get; set; }
        public Lead Lead { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Education")]
        public int EducationLevelId { get; set; }
        public Education Education { get; set; }

        public string? MessageToStudent { get; set; }

        public DateTime CheckDate { get; set; }
        public bool IsAccepted { get; set; } = false;


    }
}

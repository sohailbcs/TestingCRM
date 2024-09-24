using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.Students;
using LegalAndGeneralConsultantCRM.Models.Universiies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.ProgramsTalk
{
    public class ProgramInTalk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProgramTalkId { get; set; }
        public string ?ProgramName { get; set; }
        public int? UniversityId { get; set; }
        [ForeignKey("UniversityId")]
        public University University { get; set; }

        // Navigation property to Student
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public int? LeadId { get; set; }
        public Lead Lead { get; set; }

    }
}

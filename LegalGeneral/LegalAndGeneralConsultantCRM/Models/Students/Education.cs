using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Students
{
    public class Education
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EducationLevelId { get; set; }
        public int?  StudentId { get; set; }
        public int? LeadId { get; set; }
        public Lead Lead { get; set; }
        public string? EducationLevel { get; set; }
        public string? EducationLevelImageUrl { get; set; }
       
        public string? DegreeTitle { get; set; }
        public string? MajorSubject { get; set; }
        public string? SchoolUni { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ObjMarks { get; set; }
        public string? TotalMarks { get; set; }
        public string? Division { get; set; }
        public string? Percentage { get; set; }
        public string? Board { get; set; }
        public string? CGPA { get; set; }
        public string? title1 { get; set; }
        public string? title2 { get; set; }
        public string? title3 { get; set; }
        public string? title4 { get; set; }
        public string? title5 { get; set; }
        public string? title6 { get; set; }
        public string? attachment2 { get; set; }
        public string? attachment3 { get; set; }
        public string? attachment4 { get; set; }
        public string? attachment5 { get; set; }
        public string? attachment6 { get; set; }
 
        public List<AcademicRecord> AcademicRecords { get; set; }


    }
}

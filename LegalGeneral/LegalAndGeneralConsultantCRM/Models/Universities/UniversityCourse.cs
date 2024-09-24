 using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Universiies
{
    public class UniversityCourse // New class to represent the many-to-many relationship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UniversityCourseId { get; set; }

        public int? UniversityId { get; set; }
        public University University { get; set; }

        public int? CourseId { get; set; }
        public int? DomainId { get; set; }
        public Domain Domain { get; set; }
        public Course Course { get; set; }

        public decimal? TuitionFee { get; set; }
        public decimal? Other { get; set; }
        public string? Currency { get; set; }
        public string? Intake1 { get; set; }
        public string? EnglishProfiency { get; set; }
        public string? EaducationRequired { get; set; }
        public string? Intake2 { get; set; }
        public string? Intake3 { get; set; }
    }
}

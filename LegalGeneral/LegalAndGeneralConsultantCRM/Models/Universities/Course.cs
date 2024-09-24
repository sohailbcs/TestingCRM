 using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Students;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Universiies
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }
        public string? Name { get; set; }
        public decimal? OtherCosts { get; set; }
        public String? DurationInYears { get; set; }
        public string? Decription { get; set; }
        public ICollection<Incentive> Incentives { get; set; }  
        public ICollection<Scholarship> Scholarships { get; set; }

        public ICollection<Student> Students { get; set; }

    }
}

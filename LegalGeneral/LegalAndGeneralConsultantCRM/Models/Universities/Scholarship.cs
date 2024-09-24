 using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Universiies
{
    public class Scholarship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScholarshipId { get; set; }
        public string? Name { get; set; }
        public string? EligibilityCriteria { get; set; }
        public int? CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime? ApplicationDeadline { get; set; }

        public int? UniversityId { get; set; }
        public University University { get; set; }
    }
}

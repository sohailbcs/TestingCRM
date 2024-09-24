 using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Universiies
{
    public class Incentive
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IncentiveId { get; set; }
        public string? Description { get; set; }
        public int? CourseId { get; set; }
        public Course Course { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models
{
    public class Frenchise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FrenchiseId { get; set; }


        [Required(ErrorMessage = "Branch Name is required")]
        public string? FrenchiseName { get; set; }
        public string? City { get; set; }

        [Required(ErrorMessage = "Branch Code is required")]
        public string? FrenchiseCode { get; set; }
        public string? Description { get; set; }
    }
}

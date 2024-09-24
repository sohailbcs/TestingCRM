using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Branches
{
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BranchId { get; set; }


        [Required(ErrorMessage = "Branch Name is required")]
        public string? BranchName { get; set; }
        public string? City { get; set; }
        public string? branchType { get; set; }

        [Required(ErrorMessage = "Branch Code is required")]
        public string? BranchCode { get; set; }
        public string? Description { get; set; }
    }
}

 using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Universiies
{
    public class OfferProgram
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OfferProgramId { get; set; }
        public string Name { get; set; }
 
    }
}

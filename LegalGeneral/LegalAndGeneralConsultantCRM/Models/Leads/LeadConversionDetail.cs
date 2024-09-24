 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Leads
{
    public class LeadConversionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SoldProject { get; set; }
        public int SoldProduct { get; set; }
        public int SalePrice { get; set; }
        public int DownPayment { get; set; }
        public int BalancePayment { get; set; }
        public int ReferralId { get; set; }
        public string UserId { get; set; }
        
 
    }
}

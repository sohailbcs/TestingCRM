using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Referrals
{
    public class Referral
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReferralId { get; set; }
        public string? Name { get; set; }
        public string? Contact1 { get; set; }
        public string? Contact2{ get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public string? RefType { get; set; }
        public string? Share { get; set; }
     }
}

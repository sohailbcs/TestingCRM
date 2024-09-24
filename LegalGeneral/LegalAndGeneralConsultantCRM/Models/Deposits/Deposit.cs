using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.Students;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Deposits
{
    public class Deposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepositId { get; set; }
        public int? StudentId { get; set; }
        public string? UserId { get; set; }
        public Student Student { get; set; }
        public int? LeadId { get; set; }
        public Lead Lead { get; set; }
        public decimal? AccountNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime DepositDate { get; set; }
        public string?  DepositPayImagePath { get; set; }
        public string?  Currency { get; set; }
        public string? AccountTitle { get; set; }
    }
}

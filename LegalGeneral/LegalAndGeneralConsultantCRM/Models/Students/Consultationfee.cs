using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Students
{
    public class Consultationfee
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeeId { get; set; }
        public int?  StudentId { get; set; }
        public int? LeadId { get; set; }
        public double? Amount { get; set; }
        public string? ReceiptUrl { get; set; }
        public DateTime? Date { get; set; }
        
         

    }
}

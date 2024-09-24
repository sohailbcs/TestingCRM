using LegalAndGeneralConsultantCRM.Models.Leads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.CalendarEvents
{
    public class CalendarEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CalendarEventId { get; set; }

       
        public int? LeadId { get; set; }
        public string? UserId { get; set; }

         public Lead Lead { get; set; }

        
        public DateTime? EventDate { get; set; }

        
        public string Description { get; set; }
        public string Name { get; set; }

        
        public string ThemeColor { get; set; }
        public bool? IsRead { get; set; }
    }
}

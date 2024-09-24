using System.ComponentModel.DataAnnotations;

namespace LegalAndGeneralConsultantCRM.ViewModels
{
    public class CalendarEventViewModel
    {
        [Required]
		public int CalendarEventId { get; set; }
		public int LeadId { get; set; }

        [Required]
        public DateTime? EventDate { get; set; }

        public string Description { get; set; }

        [Required]
        public string ThemeColor { get; set; }
        public string Name { get; set; }
    }
}

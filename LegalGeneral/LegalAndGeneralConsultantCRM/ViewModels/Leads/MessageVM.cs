using LegalAndGeneralConsultantCRM.Models.ActivitiesLog;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.Students;

namespace LegalAndGeneralConsultantCRM.ViewModels.Leads
{
	public class MessageVM
	{
		public Lead Leads { get; set; }
		public IEnumerable<ActivityLog> ActivityLogs { get; set; } // Change to IEnumerable<ActivityLog>
		public StudentMessage StudentMessages { get; set; }
		
	}
}

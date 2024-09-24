using LegalAndGeneralConsultantCRM.Models;

namespace LegalAndGeneralConsultantCRM.ViewModels
{
	public class TargetViewModel
	{
		public List<TargetTask> Calls { get; set; }
		public List<TargetTask> Visits { get; set; }
		public List<TargetTask> Meetings { get; set; }
		public List<TargetTask> SalesAmount { get; set; }
		public List<TargetTask> Sales { get; set; }
		public List<TargetTask> ConnectedCalls { get; set; }
        public FollowUpCounts FollowUpCounts { get; set; }
    }
}

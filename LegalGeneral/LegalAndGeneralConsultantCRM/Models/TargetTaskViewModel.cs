using Microsoft.AspNetCore.Mvc.Rendering;

namespace LegalAndGeneralConsultantCRM.ViewModels
{
    public class TargetTaskViewModel
    {
        public string SelectedUserId { get; set; }

        public string UserFullName { get; set; }
        public string TaskType { get; set; }
        public string Description { get; set; }
        public double targetquantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string createdby { get; set; }

        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> TaskTypes { get; set; }
    }
}

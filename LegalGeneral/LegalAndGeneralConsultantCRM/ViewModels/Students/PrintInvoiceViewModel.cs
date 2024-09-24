using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.ProgramsTalk;
using LegalAndGeneralConsultantCRM.Models.Students;
using LegalAndGeneralConsultantCRM.Models.Universiies;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;
using LegalAndGeneralConsultantCRM.ViewModels.Common;

namespace LegalAndGeneralConsultantCRM.ViewModels.Students
{
    public class PrintInvoiceViewModel
    {
        public Lead Lead { get; set; }
        public Student Student  { get; set; }
        public  Consultationfee  ConsultationFees { get; set; }


    }
}

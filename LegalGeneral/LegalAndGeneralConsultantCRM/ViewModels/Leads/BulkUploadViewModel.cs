using Microsoft.AspNetCore.Http;

namespace LegalAndGeneralConsultantCRM.ViewModels.Leads
{
    public class BulkUploadViewModel
    {
        public IFormFile File { get; set; }
    }
}

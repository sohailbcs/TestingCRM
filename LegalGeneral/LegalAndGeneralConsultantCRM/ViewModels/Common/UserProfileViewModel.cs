using LegalAndGeneralConsultantCRM.Areas.Identity.Data;

namespace LegalAndGeneralConsultantCRM.ViewModels.Common
{
    public class UserProfileViewModel
    {
        public ChangePasswordViewModel ChangePasswordModel { get; set; }
        public LegalAndGeneralConsultantCRMUser CRMUserModel { get; set; }
        public IFormFile ProfileImage { get; set; }

    }
}

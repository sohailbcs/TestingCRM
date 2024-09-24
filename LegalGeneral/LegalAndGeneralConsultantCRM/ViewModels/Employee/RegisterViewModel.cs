using Microsoft.AspNetCore.Identity;

namespace LegalAndGeneralConsultantCRM.ViewModels.Employee
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public int? SelectedServiceId { get; set; }

    }
}

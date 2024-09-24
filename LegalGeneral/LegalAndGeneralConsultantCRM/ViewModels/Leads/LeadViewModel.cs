using System.ComponentModel.DataAnnotations;

namespace LegalAndGeneralConsultantCRM.ViewModels.Leads
{
    public class LeadViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "First Name must be at least 3 characters long")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ]+$", ErrorMessage = "First Name cannot be only numbers and must contain some characters")]
        public string FirstName { get; set; }

        [RegularExpression(@"^\d{11,16}$", ErrorMessage = "Phone number must be between 11 and 16 digits.")]
        [Required(ErrorMessage = "Phone Number Name is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "LastName Name must be at least 3 characters long")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ]+$", ErrorMessage = "LastName Name cannot be only numbers and must contain some characters")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Gender name is required")]
        public string? Gender { get; set; }
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        [Required(ErrorMessage = "Email name is required")]

        public string? Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Required(ErrorMessage = "Address name is required")]
        [StringLength(250, ErrorMessage = "Address must not exceed 30 characters")]
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public int? UniversityId { get; set; }
        public int? DomainId { get; set; }
        public string? Eaducation { get; set; }
        public string? YearCompletion { get; set; }
        public string? InterestedCountry { get; set; }
        public string? InterestedProgram { get; set; }
        public string? Course { get; set; }
        public string? Country { get; set; }
        public string? Notes { get; set; }
        public string? Industry { get; set; }
        [Required(ErrorMessage = "Lead Source name is required")]
        [StringLength(30,  ErrorMessage = "Lead Source must not exceed 30 characters")]
        public string? LeadSource { get; set; }
        public string? LeadSourceDetails { get; set; }
        public bool? IsLeadAssign { get; set; } = false;
       
      
        public int? ReferralId { get; set; }

    }
}

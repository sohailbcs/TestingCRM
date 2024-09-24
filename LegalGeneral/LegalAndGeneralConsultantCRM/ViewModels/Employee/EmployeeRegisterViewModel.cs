using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LegalAndGeneralConsultantCRM.ViewModels.Employee
{
    public class EmployeeRegisterViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "First Name must be at least 3 characters long")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ]+$", ErrorMessage = "First Name cannot be only numbers and must contain some characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "Last Name must be at least 3 characters long")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ]+$", ErrorMessage = "Last Name cannot be only numbers and must contain some characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Job title is required")]
        public string JobTitle { get; set; }
        public string? BranchType { get; set; }

        [Required(ErrorMessage = "Email name is required")]
        public string Email { get; set; }
        [RegularExpression(@"^\d{11,16}$", ErrorMessage = "Phone number must be between 11 and 16 digits.")]
        [Required(ErrorMessage = "Phone Number Name is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        public bool Applicationprocessor { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid zip code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        public string Notes { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Team member type is required")]
        public string TeamMemeberType { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, and one number.")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string ExistingPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Brand ID is required")]
        public int BrandId { get; set; }
    }
}

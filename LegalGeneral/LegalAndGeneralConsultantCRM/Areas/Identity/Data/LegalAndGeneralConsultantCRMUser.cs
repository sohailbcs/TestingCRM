using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegalAndGeneralConsultantCRM.Models.Branches;
using Microsoft.AspNetCore.Identity;

namespace LegalAndGeneralConsultantCRM.Areas.Identity.Data;

// Add profile data for application users by adding properties to the LegalAndGeneralConsultantCRMUser class
public class LegalAndGeneralConsultantCRMUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
  
    public string? Notes { get; set; }
    public bool? Status { get; set; }
    public bool? Applicationprocessor { get; set; }
    public string? BranchType { get; set; }
    public string? Department { get; set; }
    public string? TeamMemeberType { get; set; }
    public string? Notification { get; set; }
    public string? ProfileImage { get; set; }
    public string? EmailOrPhoneNumber { get; set; }
    public int? SelectedServiceId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int BrandId { get; set; }
    public Branch Branch { get; set; } // Navigation property

}


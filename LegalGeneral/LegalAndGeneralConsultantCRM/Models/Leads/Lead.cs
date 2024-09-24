using LegalAndGeneralConsultantCRM.Models.CalendarEvents;
using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.LeadFollowUp;
using LegalAndGeneralConsultantCRM.Models.Referrals;
using LegalAndGeneralConsultantCRM.Models.Students;
using LegalAndGeneralConsultantCRM.Models.Universiies;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Leads
{
    public class Lead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeadId { get; set; }

        public string? Applicantstatus { get; set; }
        public string? StudyGap { get; set; }
        public string? Gender { get; set; }
        public string? Cnic { get; set; }
        public string? FirstName { get; set; }
        public string? SourceId { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? PhoneNumber { get; set; }
        public string? PhoneNumber2 { get; set; }
        public string? Eaducation { get; set; }
        public string? yearCompletion { get; set; }
        public string? interestedcountry { get; set; }
        public string? interestedprogram { get; set; }
        public int? UniversityId { get; set; } // Add this for the relationship
        public University University { get; set; } // Navigation property

        public int? DomainId { get; set; } // Add this for the relationship
        public Domain Domain { get; set; } // Navigation property


        public string? LastName { get; set; }
      
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? Notes { get; set; }
        public string? Industry { get; set; }
        public string? LeadSource { get; set; }
        public string? LeadSourceDetails { get; set; }
        public bool? IsLeadAssign { get; set; } = false;
        public bool? IsEnrolled { get; set; } = false;
       
        public int? ReferralId { get; set; }
        public string? UserId { get; set; }
        public Referral Referral { get; set; }

        public ICollection<LeadAssignEmployee> Assignees { get; set; }
        public ICollection<FollowUp> FollowUps { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<UniversityCourse> UniversityCourse { get; set; }
        public ICollection<CalendarEvent> CalendarEvents { get; set; }
        // Add the navigation property for StudentMessages
        public ICollection<StudentMessage> StudentMessages { get; set; }

        // Add the navigation property for Deposits
        public ICollection<Deposit> Deposits { get; set; }
        public ICollection<VisaApplication> VisaApplications { get; set; }

    }
}

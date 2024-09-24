using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.ProgramsTalk;
using LegalAndGeneralConsultantCRM.Models.Universiies;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Students
{
    public class Student
    {
        // Primary key for the student entity
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }
        public int? LeadId { get; set; }
        public Lead Lead { get; set; }
        // Basic information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? Applicantstatus { get; set; }
        public string? StudyGap { get; set; }
        public string Gender { get; set; }
        public string Cnic { get; set; }
        public string Domicile { get; set; }
        public bool IsDisable { get; set; }
        public string? UserId { get; set; }
        // Additional details
        public bool IsInternationalStudent { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Nationality { get; set; }
        public string? Major { get; set; }
        public string? AdditionalInformation { get; set; }

        // Emergency contact information
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }

        // Guardian information
        public string? GuardianName { get; set; }
        public string? GuardianEmail { get; set; }
        public string? GuardianPhoneNumber { get; set; }
        public string? GuardianAddress { get; set; }
        public string? Relation { get; set; }

        // Health-related information
        public string? BloodGroup { get; set; }

        // Language information
        public string Language { get; set; }
        public string? EmergencyAddress { get; set; }
        public string? EmergencyPhoneNumber { get; set; }
        public string? EmergencyName { get; set; }
         public string? TemporaryAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Domains { get; set; }
        public string? Intake { get; set; }
        public int? UniversityId { get; set; }
        public University University { get; set; }

        public int? CourseId { get; set; }
        public Course Course { get; set; }

        public List<Domain> Domain { get; set; }
        public List<Education> Education { get; set; }
        public List<AcademicRecord> AcademicRecords { get; set; }
        public List<ProgramInTalk> ProgramInTalks { get; set; }
        public List<UniversityCourse> UniversityCourse { get; set; }
        public List<Deposit> Deposits { get; set; }
        public List<VisaApplication> VisaApplications { get; set; }
        public List<StudentMessage> StudentMessages { get; set; }
        

    }
}

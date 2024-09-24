using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.ProgramsTalk;
using LegalAndGeneralConsultantCRM.Models.Students;
using LegalAndGeneralConsultantCRM.Models.Universiies;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;

namespace LegalAndGeneralConsultantCRM.ViewModels.Students
{
    public class StudentProgramViewModel
    {
       public string? Applicantstatus { get; set; }
        public string? StudyGap { get; set; }
        public Student Students { get; set; }
        public Consultationfee  Fees { get; set; }
        public Education Educations { get; set; }
        public AcademicRecord AcademicRecords { get; set; }
        public University Universities{ get; set; }
        public OfferProgram OfferPrograms { get; set; }
        public string SelectedProgram { get; set; } 
        public Deposit Deposits { get; set; } 
        public VisaApplication VisaApplications { get; set; } 
        public StudentMessage StudentMessages { get; set; }
        public StudentEnrollment StudentEnrollments { get; set; }
        public Course Course { get; set; } // For binding Course details
        public UniversityCourse UniversityCourse { get; set; }
        public string? Domains { get; set; }
        public string? Intake { get; set; }

        public List<Education> EducationsList { get; set; }
        public List<OfferProgram> OfferProgramList { get; set; }
        public List<VisaApplication> VisaApplicationList { get; set; }
        public List<ProgramInTalk> ProgramInTalkList { get; set; }
        public List<Deposit> DepositList { get; set; }

        public IFormFile ReceiptFile { get; set; } // File upload property
        public IFormFile EducationImageFile { get; set; }   
        public IFormFile DepositPayImagePath { get; set; }   
        public IFormFile attachment2ImagePath { get; set; }     
        public IFormFile attachment3ImagePath { get; set; }     
        public IFormFile attachment4ImagePath { get; set; }     


    }
}

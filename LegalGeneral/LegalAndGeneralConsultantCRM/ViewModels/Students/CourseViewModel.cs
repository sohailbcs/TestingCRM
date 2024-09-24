namespace LegalAndGeneralConsultantCRM.ViewModels.Students
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public int UniversityId { get; set; } // Add this property to specify which university the course is related to
        public decimal TuitionFee { get; set; }
        public int? DurationInYears { get; set; }
    }

}

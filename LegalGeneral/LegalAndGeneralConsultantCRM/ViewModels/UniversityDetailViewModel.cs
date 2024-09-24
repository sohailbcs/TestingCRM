
namespace LegalAndGeneralConsultantCRM.ViewModels
{
    public class UniversityDetailViewModel
    {
        public UniversityViewModel University { get; set; }
        public List<CourseViewModel> UniversityCourses { get; set; }
    }

    public class UniversityViewModel
    {
        public int UniversityId { get; set; }
        public string Name { get; set; }
        public string? Website { get; internal set; }
        public string? ContactPhone { get; internal set; }
        public string? ContactEmail { get; internal set; }
        public string? StreetAddress { get; internal set; }
        public string? Country { get; internal set; }
        public DateTime? Founded { get; internal set; }
        public string? Chancellor { get; internal set; }
        public string? Ranking { get; internal set; }
        // Other university properties
    }

    public class CourseViewModel
    {
        public int UniversityId { get; set; } // Ensure this is added

        public int CourseId { get; set; }
        public string Name { get; set; }
        public decimal TuitionFee { get; set; }
        public string? DurationInYears { get; set; } // Include DurationInYears property
    }

}

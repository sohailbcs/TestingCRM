using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegalAndGeneralConsultantCRM.Models.Universiies
{
    public class University
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UniversityId { get; set; }
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }
        public string? Location { get; set; }
        [Required]
        public string? Country { get; set; }
        [MinLength(3, ErrorMessage = "Address must be at least 3 characters long.")]
        [MaxLength(200, ErrorMessage = "Address cannot be more than 200 characters.")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Description { get; set; }
        public DateTime? Founded { get; set; }
        [Url(ErrorMessage = "Invalid URL")]
        public string? Website { get; set; }
        public string? ContactEmail { get; set; }
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number can only contain digits.")]

        public string? ContactPhone { get; set; }
        public string? Type { get; set; }
        public string? Chancellor { get; set; }
        public string? Ranking { get; set; }
        public string? ResearchAreas { get; set; }
        public bool? OffersInternationalPrograms { get; set; }
        public string? OfferProgram { get; set; }

        public ICollection<UniversityCourse> UniversityCourses { get; set; }
        public ICollection<Scholarship> Scholarships { get; set; }


    }
}

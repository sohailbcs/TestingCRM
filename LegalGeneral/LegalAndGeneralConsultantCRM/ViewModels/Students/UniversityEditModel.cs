namespace LegalAndGeneralConsultantCRM.ViewModels.Students
{
    public class UniversityEditModel
    {
        public int UniversityId { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string StreetAddress { get; set; }
        public string Country { get; set; }
        public DateTime? Founded { get; set; } // Use nullable DateTime if the field is optional
        public string Chancellor { get; set; }
        public string Ranking { get; set; }
    }
}

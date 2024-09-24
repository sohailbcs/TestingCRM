namespace LegalAndGeneralConsultantCRM.Models
{
    public class TargetTask
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string TaskType { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double targetquantity { get; set; }
        public string createdby { get; set; }
        public DateTime createdon { get; set; }=  DateTime.Now;
       

        
    }
}

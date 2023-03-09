namespace CBT.DAL
{
    public class CommonEntity
    {
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Guid ClientId { get; set; }
        public int UserType { get; set; }
        public string SmsClientId { get; set; }
    }
}
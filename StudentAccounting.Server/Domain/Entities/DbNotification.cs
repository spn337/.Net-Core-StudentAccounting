namespace StudentAccounting.Server.Domain.Entities
{
    public class DbNotification
    {
        public string Id { get; set; }
        public string HangfireJobId { get; set; }
        public string EnrollmentId { get; set; }

    }
}

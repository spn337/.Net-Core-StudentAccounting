namespace StudentAccounting.RazorClassLib.Views.Emails.Notification
{
    public class NotificationViewModel
    {
        public string CourseName { get; set; }
        public int DaysToStudyCount { get; set; }

        public NotificationViewModel(string courseName, int daysToStudyCount)
        {
            CourseName = courseName;
            DaysToStudyCount = daysToStudyCount;
        }
    }
}

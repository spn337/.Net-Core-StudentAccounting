namespace StudentAccounting.RazorClassLib.Views.Emails.ConfirmAccount
{
    public class ConfirmAccountViewModel
    {
        public ConfirmAccountViewModel(string confirmEmailUrl)
        {
            ConfirmEmailUrl = confirmEmailUrl;
        }
        public string ConfirmEmailUrl { get; set; }
    }
}

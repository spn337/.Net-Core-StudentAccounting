using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using StudentAccounting.RazorClassLib.Services;
using StudentAccounting.RazorClassLib.Views.Emails.ConfirmAccount;
using StudentAccounting.RazorClassLib.Views.Emails.Notification;
using System;
using System.Threading.Tasks;


namespace StudentAccounting.MailService
{
    public class EmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly IConfiguration configuration;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;

        public EmailService(
            ILogger<EmailService> logger,
            IConfiguration configuration,
            IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public async Task<string> GetConfirmAccountTemplateAsync(string confirmationLink)
        {
            var viewModel = new ConfirmAccountViewModel(confirmationLink);
            string viewName = "/Views/Emails/ConfirmAccount/ConfirmAccount.cshtml";
            var result = await razorViewToStringRenderer.RenderViewToStringAsync(viewName, viewModel);
            return result;
        }
        public async Task<string> GetNotificationTemplateAsync(string courseName, int daysToStudyCount)
        {
            var viewModel = new NotificationViewModel(courseName, daysToStudyCount);
            string viewName = "/Views/Emails/Notification/Notification.cshtml";
            var result = await razorViewToStringRenderer.RenderViewToStringAsync(viewName, viewModel);
            return result;
        }
        public bool SendEmail(string email, string subject, string body)
        {
            try
            {
                var smtp = new SmtpGoogleServer()
                {
                    Host = configuration["StudyDate:SmtpGoogleServer:Host"],
                    Port = Convert.ToInt32(configuration["StudyDate:SmtpGoogleServer:Port"]),
                    UseSSL = Convert.ToBoolean(configuration["StudyDate:SmtpGoogleServer:UseSSL"]),
                    Login = configuration["StudyDate:SmtpGoogleServer:Login"],
                    Password = configuration["StudyDate:SmtpGoogleServer:Password"]
                };

                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Pro Courses", smtp.Login));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = body
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(smtp.Host, smtp.Port, smtp.UseSSL);
                    client.Authenticate(smtp.Login, smtp.Password);
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return false;
            }
        }
    }
}

using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentAccounting.MailService;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.Domain;
using StudentAccounting.Server.Domain.Entities;
using StudentAccounting.Server.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace StudentAccounting.Server.Services
{
    public interface IAuthService
    {
        bool CheckPassword(DbUser user, string password);
        SignInResult Login(string email, string password, bool isRememberMe);
        void Logout();
        IList<string> GetUserRoles(DbUser user);

        string GenerateJwtToken(DbUser user, IList<string> roles);
        IdentityResult ConfirmEmail(DbUser user, string token);

        bool SendConfirmAccountEmail(DbUser user);
        bool CreateNotificationEmails(DbEnrollment enrollment);
    }


    public class AuthService : IAuthService
    {
        private readonly UserManager<DbUser> userManager;
        private readonly SignInManager<DbUser> signInManager;
        private readonly EmailService emailService;
        private readonly AppDbContext context;
        private readonly JwtBearerTokenSettings jwtBearerTokenSettings;
        public AuthService(
            UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            EmailService emailService,
            AppDbContext context,
            IOptions<JwtBearerTokenSettings> jwtBearerTokenOptions)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.context = context;
            jwtBearerTokenSettings = jwtBearerTokenOptions.Value;
        }

        public bool CheckPassword(DbUser user, string password)
            => CheckUserPasswordAsync(user, password).Result;
        public IList<string> GetUserRoles(DbUser user)
            => GetUserRolesAsync(user).Result;
        public SignInResult Login(string email, string password, bool isRememberMe)
            => LoginAsync(email, password, isRememberMe).Result;

        public void Logout()
            => LogoutAsync();

        public string GenerateJwtToken(DbUser user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            List<Claim> claims = new List<Claim>()
            {
                new Claim(Names.ID, user.Id),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(Names.ROLE, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddSeconds(jwtBearerTokenSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(
                    jwtBearerTokenSettings.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IdentityResult ConfirmEmail(DbUser user, string token)
           => ConfirmEmailAsync(user, token).Result;

        public bool SendConfirmAccountEmail(DbUser user)
            => SendConfirmAccountEmailAsync(user).Result;

        public bool CreateNotificationEmails(DbEnrollment enrollment)
           => CreateNotificationEmailsAsync(enrollment).Result;



        private async Task<bool> CheckUserPasswordAsync(DbUser user, string password)
            => await userManager.CheckPasswordAsync(user, password);
        public async Task<IList<string>> GetUserRolesAsync(DbUser user)
            => await userManager.GetRolesAsync(user);
        private async Task<SignInResult> LoginAsync(string email, string password, bool isRememberMe)
            => await signInManager.PasswordSignInAsync(email, password, isRememberMe, false);
        private async void LogoutAsync()
            => await signInManager.SignOutAsync();
        private async Task<IdentityResult> ConfirmEmailAsync(DbUser user, string token)
        {
            var tokenDecodedBytes = WebEncoders.Base64UrlDecode(token);
            var decoderToken = Encoding.UTF8.GetString(tokenDecodedBytes);

            var result = await userManager.ConfirmEmailAsync(user, decoderToken);
            return result;
        }
        private async Task<bool> SendConfirmAccountEmailAsync(DbUser user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
            var encoderToken = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

            var baseUrl = $"{ClientUrls.CONFIRM_EMAIL}";

            var confirmationLink = $"{baseUrl}?userId={user.Id}&token={encoderToken}";

            var body = await emailService.GetConfirmAccountTemplateAsync(confirmationLink);
            if (body == null)
            {
                return false;
            }

            var result = emailService.SendEmail(user.Email, "Confirm your account", body);
            return result;
        }
        private async Task<bool> CreateNotificationEmailsAsync(DbEnrollment enrollment)
        {
            var user = enrollment.User;
            var course = enrollment.Course;
            var studyDate = enrollment.StudyDate;


            var notificationDays = new DateTime[] {
                studyDate.AddMonths(-1),
                studyDate.AddDays(-7),
                studyDate.AddDays(-1)
            };

            var body = await emailService.GetNotificationTemplateAsync(course.Name, studyDate.GetDaysToStudy());
            if (body == null)
            {
                return false;
            }

            BackgroundJob.Enqueue(() => emailService.SendEmail(user.Email, "Notification", body));

            for (int i = 0; i < notificationDays.Length; i++)
            {
                if (notificationDays[i] > DateTime.UtcNow)
                {
                    var arrayDate = new int[] { 30, 7, 1 };
                    body = await emailService.GetNotificationTemplateAsync(course.Name, arrayDate[i]);
                    if (body == null)
                    {
                        return false;
                    }

                    var jobId = BackgroundJob.Schedule(
                        () => emailService.SendEmail(user.Email, "Notification", body),
                        notificationDays[i]);

                    DbNotification notification = new DbNotification
                    {
                        EnrollmentId = enrollment.Id,
                        HangfireJobId = jobId
                    };

                    context.Notifications.Add(notification);
                    context.SaveChanges();
                }
            }
            return true;
        }
    }
}

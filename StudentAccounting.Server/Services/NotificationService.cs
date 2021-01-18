using Hangfire;
using StudentAccounting.Server.Domain;
using StudentAccounting.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAccounting.Server.Services
{
    public interface INotificationService
    {
        void DeleteNotifications(DbEnrollment enrollment);
    }

    public class NotificationService : INotificationService
    {
        private readonly AppDbContext context;

        public NotificationService(AppDbContext context)
        {
            this.context = context;
        }


        public void DeleteNotifications(DbEnrollment enrollment)
        {
            if (enrollment != null)
            {
                var notifications = context.Notifications.Where(e => e.EnrollmentId == enrollment.Id);
                if (notifications.Count() != 0)
                {
                    foreach (var notification in notifications)
                    {
                        BackgroundJob.Delete(notification.HangfireJobId);
                        context.Notifications.Remove(notification);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}

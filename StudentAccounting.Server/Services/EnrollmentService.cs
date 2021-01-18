using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentAccounting.Server.Domain.Entities;
using System;
using System.Linq;
using StudentAccounting.Server.Domain;


namespace StudentAccounting.Server.Services
{
    public interface IEnrollmentService
    {
        IQueryable<DbEnrollment> GetEnrollments(string userId);
        IQueryable<DbCourse> GetUnsubscribedCourses(string userId);

        DbEnrollment GetEnrollmentById(string userId, string courseId);
        DbEnrollment GetEnrollmentByUserId(string userId);

        bool IsUserExist(string userId);
        bool IsCourseExist(string courseId);
        bool IsEnrollmentExist(string userId, string courseId);
        bool IsEnrollmentExistByUserId(string userId);

        DbEnrollment Subscribe(string userId, string courseId, DateTime studyDate);
        void Unsubscribe(DbEnrollment enrollment);
    }


    public class EnrollmentService : IEnrollmentService
    {
        private readonly UserManager<DbUser> userManager;
        private readonly AppDbContext context;

        public EnrollmentService(UserManager<DbUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        public IQueryable<DbEnrollment> GetEnrollments(string userId)
            => context.Enrollments.Where((enrollment) => enrollment.UserId == userId)
                                  .Include(course => course.Course);

        public IQueryable<DbCourse> GetUnsubscribedCourses(string userId)
            => context.Courses.Where(course => !course.Enrollments.Any(env => env.UserId == userId));

        public DbEnrollment GetEnrollmentById(string userId, string courseId)
            => context.Enrollments.SingleOrDefault(enrollment => enrollment.CourseId == courseId && enrollment.UserId == userId);

        public DbEnrollment GetEnrollmentByUserId(string userId)
           => context.Enrollments.SingleOrDefault(enrollment => enrollment.UserId == userId);

        public bool IsUserExist(string userId)
            => userManager.Users.SingleOrDefault(user => user.Id == userId) != null;
        public bool IsCourseExist(string courseId)
            => context.Courses.SingleOrDefault(course => course.Id == courseId) != null;
        public bool IsEnrollmentExist(string userId, string courseId)
            => GetEnrollmentById(userId, courseId) != null;
        public bool IsEnrollmentExistByUserId(string userId)
            => GetEnrollmentByUserId(userId) != null;

        public DbEnrollment Subscribe(string userId, string courseId, DateTime studyDate)
        {
            var enrollment = new DbEnrollment
            {
                UserId = userId,
                CourseId = courseId,
                StudyDate = studyDate
            };
            context.Enrollments.Add(enrollment);
            context.SaveChanges();

            return enrollment;
        }

        public void Unsubscribe(DbEnrollment enrollment)
        {
            context.Enrollments.Remove(enrollment);
            context.SaveChanges();
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentAccounting.Server.Domain.Entities;

namespace StudentAccounting.Server.Domain
{
    public class AppDbContext : IdentityDbContext<DbUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<DbCourse> Courses { get; set; }
        public DbSet<DbEnrollment> Enrollments { get; set; }
        public DbSet<DbNotification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<DbCourse>()
                        .ToTable("Courses");

            modelBuilder.Entity<DbCourse>()
                        .Property(course => course.Id)
                        .ValueGeneratedOnAdd();


            modelBuilder.Entity<DbEnrollment>()
                        .ToTable("Enrollments");

            modelBuilder.Entity<DbEnrollment>()
                        .HasKey(enrollment => new { enrollment.Id });

            modelBuilder.Entity<DbEnrollment>()
                        .Property(enrollment => enrollment.Id)
                        .ValueGeneratedOnAdd();


            modelBuilder.Entity<DbNotification>()
                        .ToTable("Notifications");

            modelBuilder.Entity<DbNotification>()
                        .Property(notification => notification.Id)
                        .ValueGeneratedOnAdd();

            modelBuilder.Entity<DbNotification>()
                        .Property(notification => notification.HangfireJobId)
                        .IsRequired();

            modelBuilder.Entity<DbNotification>()
                       .Property(notification => notification.EnrollmentId)
                       .IsRequired();
        }
    }
}

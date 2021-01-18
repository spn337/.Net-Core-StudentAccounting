using System;

namespace StudentAccounting.Server.Domain.Entities
{
    public class DbEnrollment
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public DateTime StudyDate { get; set; }

        public virtual DbUser User { get; set; }
        public virtual DbCourse Course { get; set; }

    }
}

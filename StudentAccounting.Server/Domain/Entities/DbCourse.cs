using System.Collections.Generic;

namespace StudentAccounting.Server.Domain.Entities
{
    public class DbCourse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<DbEnrollment> Enrollments { get; set; }
    }
}

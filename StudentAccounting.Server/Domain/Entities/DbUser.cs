using Microsoft.AspNetCore.Identity;
using System;

namespace StudentAccounting.Server.Domain.Entities
{
    public class DbUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public DateTime RegisteredDate { get; set; }

        public DbEnrollment Enrollment { get; set; }
    }
}

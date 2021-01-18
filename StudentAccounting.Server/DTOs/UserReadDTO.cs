using StudentAccounting.Server.Domain.Entities;
using System.Collections.Generic;

namespace StudentAccounting.Server.DTOs
{
    public class UserReadDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string RegisteredDate { get; set; }
        public UserCourseReadDTO Course { get; set; }
    }

}

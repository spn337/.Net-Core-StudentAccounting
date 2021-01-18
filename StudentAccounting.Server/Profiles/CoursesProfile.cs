using AutoMapper;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.Domain.Entities;
using StudentAccounting.Server.DTOs;
using System.Linq;

namespace StudentAccounting.Server.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<DbCourse, CourseReadDTO>();
        }
    }
}

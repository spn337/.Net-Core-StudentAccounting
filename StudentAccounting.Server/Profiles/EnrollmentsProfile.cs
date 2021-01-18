using AutoMapper;
using StudentAccounting.Server.Domain.Entities;
using StudentAccounting.Server.DTOs;
using StudentAccounting.Server.Helpers;


namespace StudentAccounting.Server.Profiles
{
    public class EnrollmentsProfile : Profile
    {
        public EnrollmentsProfile()
        {
            CreateMap<DbEnrollment, SubsCourseReadDTO>()
                 .ForMember(dto => dto.Id,
                           opt => opt.MapFrom(enrollment => enrollment.Course.Id))
                 .ForMember(dto => dto.Name,
                           opt => opt.MapFrom(enrollment => enrollment.Course.Name))
                 .ForMember(dto => dto.Description,
                           opt => opt.MapFrom(enrollment => enrollment.Course.Description))
                 .ForMember(dto => dto.StudyDate,
                           opt => opt.MapFrom(enrollment => enrollment.StudyDate.ToString("MM/dd/yyyy")))
                 .ForMember(dto => dto.DaysToStudyCount,
                           opt => opt.MapFrom(enrollment => enrollment.StudyDate.GetDaysToStudy()));

            
        }
    }
}

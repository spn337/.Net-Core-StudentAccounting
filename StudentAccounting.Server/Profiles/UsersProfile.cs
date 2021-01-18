using AutoMapper;
using StudentAccounting.Server.Domain.Entities;
using StudentAccounting.Server.DTOs;
using System;
using StudentAccounting.Server.Constants;


namespace StudentAccounting.Server.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<DbUser, UserReadDTO>()
                .ForMember(dto => dto.RegisteredDate,
                           opt => opt.MapFrom(user => user.RegisteredDate.ToString(Format.DATETIME_TOSTRING)));

            CreateMap<DbUser, UserProfileReadDTO>();
               
            CreateMap<UserCreateDTO, DbUser>()
                .ForMember(user => user.UserName,
                           opt => opt.MapFrom(dto => dto.Email))
                .ForMember(user => user.RegisteredDate,
                           opt => opt.MapFrom(dto => DateTime.UtcNow));

            CreateMap<UserUpdateDTO, DbUser>()
                .ForMember(user => user.UserName,
                           opt => opt.MapFrom(dto => dto.Email))
                .ForMember(user => user.RegisteredDate,
                           opt => opt.MapFrom(dto => DateTime.UtcNow));
        }
    }
}

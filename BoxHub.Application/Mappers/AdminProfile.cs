using AutoMapper;
using BoxHub.Application.DTOs.Responses.Admins;
using BoxHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Mappers
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<user, UserItemResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.user_id))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.role.ToString()))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.user_status.ToString()))
                .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.user_profile!.first_name))
                .ForMember(dest => dest.LastName,
                    opt => opt.MapFrom(src => src.user_profile!.last_name))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.user_profile!.gender))
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.user_profile!.date_of_birth));
        }
    }
}

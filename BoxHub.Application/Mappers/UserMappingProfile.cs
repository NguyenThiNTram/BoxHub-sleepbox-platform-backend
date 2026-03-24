using AutoMapper;
using BoxHub.Application.DTOs.Responses.Users;
using BoxHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<user, UserProfileResponse>()
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.user_id))
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.Phone,
                    opt => opt.MapFrom(src => src.phone))
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.role.ToString().ToUpperInvariant()))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.created_at))
                .ForMember(dest => dest.LastLoginAt,
                    opt => opt.MapFrom(src => src.last_login_at))
                .ForMember(dest => dest.UserStatus,
                    opt => opt.MapFrom(src => src.user_status.ToString()))
                .ForMember(dest => dest.IsEmailVerified,
                    opt => opt.MapFrom(src => src.is_email_verified))
                .ForMember(dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.user_profile != null ? src.user_profile.first_name : null))
                .ForMember(dest => dest.LastName,
                    opt => opt.MapFrom(src => src.user_profile != null ? src.user_profile.last_name : null))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.user_profile != null ? src.user_profile.gender : null))
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.user_profile.date_of_birth))
                .ForMember(dest => dest.AvatarUrl,
                    opt => opt.MapFrom(src => src.user_profile != null ? src.user_profile.avatar_url : null));
        }
    }
}

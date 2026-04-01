using AutoMapper;
using BoxHub.Application.DTOs.Responses.Moderators;
using BoxHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Mappers
{
    public class ModeratorProfile : Profile
    {
        public ModeratorProfile()
        {
            CreateMap<user, SuspendAccountResult>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.user_id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.NewStatus, opt => opt.MapFrom(src => src.user_status))
                .ForMember(dest => dest.SuspendedById, opt => opt.Ignore())
                .ForMember(dest => dest.SuspendedAt, opt => opt.Ignore());
        }
    }
}

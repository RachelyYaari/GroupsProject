using AutoMapper;
using DAL.Dtos;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Profiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<GroupDto, Group>();
            CreateMap<Group, GroupDto>();
        }

    }
}

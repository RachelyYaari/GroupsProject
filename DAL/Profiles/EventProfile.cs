using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Models.Models;
using DAL.Dtos;


namespace DAL.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventDto, Event>();
            CreateMap<Event, EventDto>();
        }
    }
}

using DAL.Interface;
using AutoMapper;
using DAL.Dtos;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class EventData : IEvent
    {
        private readonly GroupsContext _context;
        private readonly IMapper _mapper;
        private readonly IGroup _group;
        public EventData(GroupsContext context, IMapper mapper, IGroup group)
        {
            _context = context;
            _mapper = mapper;
            _group = group;
        }

        public async Task<Event> getEventById(int eventId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            return eventEntity;
        }
        public async Task<bool> createEvent(EventDto _event,int groupId)
        {
            Event @event = _mapper.Map<Event>(_event);
            @event.EventGroup = await _group.getGroupById(groupId);
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> deleteEvent(int eventId)
        {
            var @event = _context.Events.Find(eventId);
            if (@event == null)
            {
                throw new NotImplementedException();
            }

            _context.Events.Remove(@event);
            return _context.SaveChanges() >0;
        }
    }
}

using AutoMapper;
using DAL.Dtos;
using DAL.Interface;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class GroupData : IGroup
    {
        private readonly GroupsContext _context;
        private readonly IMapper _mapper;

        public GroupData(GroupsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Group> getGroupById(int groupId)
        {
            Group groupEntity = await _context.Groups.FindAsync(groupId);
            return groupEntity;
        }


        public async Task<List<EventDto>> getAllEvents(int groupId)
        {
            var groupEntity = await _context.Groups
                .Include(g => g.Events)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (groupEntity == null)
            {
                throw new ArgumentException($"No group found with ID {groupId}", nameof(groupId));
            }
            var eventDtos = groupEntity.Events.Select(e => _mapper.Map<EventDto>(e)).ToList();
            return eventDtos;
        }


        public async Task<bool> addEventToGroup(int groupId, EventDto newEvent)
        {
            Group @group = await getGroupById(groupId);
            if (@group == null)
                return false;

            Event @event = _mapper.Map<Event>(newEvent);
            await _context.Events.AddAsync(@event);
            @event.EventGroup = @group;

            if (@group.Events == null)
                @group.Events = new List<Event>();

            @group.Events.Add(@event);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> addUserToGroup(int groupId, int userId)
        {
            Group @group = await getGroupById(groupId);
            User @user = await _context.Users.FindAsync(userId);
            if (@group == null || @user == null)
                return false;
            if (@group.Members == null)
                @group.Members = new List<User>();
            if (@user.Groups == null)
                @user.Groups = new List<Group>();
            @group.Members.Add(@user);
            @user.Groups.Add(@group);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> createGroup(GroupDto _group,int managerId)
        {
            Group @group = _mapper.Map<Group>(_group);
            @group.Manager = managerId;
            _context.Groups.Add(@group);
            await addUserToGroup( @group.Id,managerId);
            var isOkay=await _context.SaveChangesAsync();
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
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> deleteGroup(int groupId)
        {
            var group = _context.Groups.Find(groupId);
            if (group == null)
            {
                throw new NotImplementedException();
            }

            _context.Groups.Remove(group);
            return _context.SaveChanges() > 0;

        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using Models.Models;
using AutoMapper;
using DAL.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class UserData : IUser
    {
        private readonly GroupsContext _context;
        private readonly IMapper _mapper;
        public UserData(GroupsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> getUserById(int userId)
        {
            User userEntity = await _context.Users.FindAsync(userId);
            return userEntity;
        }

        public async Task<bool> createUser(UserDto _user)
        {
            await _context.Users.AddAsync(_mapper.Map<User>(_user));
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> addEvent(int userId, int eventId)
        {
            User @user = await getUserById(userId);
            Event @event = await _context.Events.FindAsync(eventId);
            if (@user == null || @event == null)
                return false;
            if (@user.Events == null)
                @user.Events = new List<Event>();
            @user.Events.Add(@event);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<UserDto>> getAllUsers(int groupId)
        {
            var userEntity = await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (userEntity == null)
            {
                throw new ArgumentException($"No group found with ID {groupId}", nameof(groupId));
            }
            var userDto = userEntity.Members.Select(e => _mapper.Map<UserDto>(e)).ToList();
            return userDto;
        }

        public async Task<List<EventDto>> getAllEvents(int userId)
        {
            var userEntity = await _context.Groups
                .Include(p => p.Events)
                .FirstOrDefaultAsync(p => p.Id == userId);

            if (userEntity == null)
            {
                throw new ArgumentException($"No group found with ID {userId}", nameof(userId));
            }
            var eventDtos = userEntity.Events.Select(e => _mapper.Map<EventDto>(e)).ToList();
            return eventDtos;
        }

        public  async Task<UserDto> getUserByEmail(string email)
        {
            User mailFind = _context.Users.FirstOrDefault(b => b.Email == email);
            UserDto afterMapper = _mapper.Map<UserDto>(mailFind);
            return afterMapper;
        }

        public async Task<bool> deleteUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new NotImplementedException();
            }

            _context.Users.Remove(user);
            return _context.SaveChanges() > 0;

        }
    }
}


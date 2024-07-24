using DAL.Dtos;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IGroup
    {
        Task<Group> getGroupById(int id);
        Task<List<EventDto>> getAllEvents(int id);

        Task<bool> createGroup(GroupDto _group, int manngerId);

        Task<bool> addEventToGroup(int groupId, EventDto newEvent);
        Task<bool> addUserToGroup(int groupId, int UserId);
        Task<bool> deleteGroup(int groupId);
    }
}

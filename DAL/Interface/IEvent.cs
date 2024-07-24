using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;
using DAL.Dtos;
namespace DAL.Interface
{
    public interface IEvent
    {
        Task<Event> getEventById(int eventId);
        Task<bool> createEvent(EventDto _event,int groupId);
        Task<bool> deleteEvent(int eventId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoDAL
{
    public class EventDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<EventModel> AllEvent()
        {
            try
            {
                var evt = objdb.Events.ToList();
                return evt.Select(x => new EventModel
                {
                    EventId = x.EventId,
                    EventName = x.EventName,
                    Description = x.Description,
                    EventDate = x.EventDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    AgentSeat = x.AgentSeat,
                    IsGuestRegistration = x.IsGuestRegistration,
                    GuestSeat = x.GuestSeat,
                    RSVP_By = x.RSVP_By,
                    RSVP_Time = x.RSVP_Time,
                    CreatedBy = x.CreatedBy,
                    CountAgent=objdb.AgentEventParticipates.Where(y=>y.EventId==x.EventId).ToList().Count(),
                    CountGuest= objdb.Guests.Where(y => y.EventId == x.EventId).ToList().Count(),

                }).OrderByDescending(x => x.EventId).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditEvent(EventModel objmodel)
        {
            try
            {

                if (objmodel.EventId == 0)
                {
                    Event objEvent = new Event
                    {
                        EventId = objmodel.EventId,
                        EventName = objmodel.EventName,
                        Description = objmodel.Description,
                        EventDate = objmodel.EventDate,
                        StartTime = objmodel.StartTime,
                        EndTime = objmodel.EndTime,
                        AgentSeat = objmodel.AgentSeat,

                        IsGuestRegistration = objmodel.IsGuestRegistration,
                        GuestSeat = objmodel.GuestSeat,
                        RSVP_By = objmodel.RSVP_By,
                        RSVP_Time = objmodel.RSVP_Time,
                        CreatedBy = objmodel.CreatedBy,
                    };
                    objdb.Events.Add(objEvent);
                    objdb.SaveChanges();
                    return objEvent.EventId;
                }
                else
                {
                    var objEvent = objdb.Events.Find(objmodel.EventId);
                    objEvent.EventName = objmodel.EventName;
                    objEvent.EventId = objmodel.EventId;
                    objEvent.Description = objmodel.Description;
                    objEvent.EventDate = objmodel.EventDate;
                    objEvent.StartTime = objmodel.StartTime;
                    objEvent.EndTime = objmodel.EndTime;
                    objEvent.IsGuestRegistration = objmodel.IsGuestRegistration;
                    objEvent.GuestSeat = objmodel.GuestSeat;

         
                    objdb.SaveChanges();
                    return objmodel.EventId;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public EventModel GetEventById(int id)
        {
            try
            {
                return objdb.Events.Where(x => x.EventId == id).Select(x => new EventModel
                {
                    EventId = x.EventId,
                    EventName = x.EventName,
                    Description = x.Description,
                    EventDate = x.EventDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    AgentSeat = x.AgentSeat,

                    IsGuestRegistration = x.IsGuestRegistration,
                    GuestSeat = x.GuestSeat,
                    RSVP_By = x.RSVP_By,
                    RSVP_Time = x.RSVP_Time,
                    CreatedBy = x.CreatedBy,
                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<EventModel> GetEventListById(int id)
        {
            try
            {
                return objdb.Events.Where(x => x.EventId == id).Select(x => new EventModel
                {
                    EventId = x.EventId,
                    EventName = x.EventName,
                    Description = x.Description,
                    EventDate = x.EventDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    AgentSeat = x.AgentSeat,

                    IsGuestRegistration = x.IsGuestRegistration,
                    GuestSeat = x.GuestSeat,
                    RSVP_By = x.RSVP_By,
                    RSVP_Time = x.RSVP_Time,
                    CreatedBy = x.CreatedBy,
                }).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


    }
}


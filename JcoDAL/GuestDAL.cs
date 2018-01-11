using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoDAL
{
   public class GuestDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<GuestModel> AllGuest()
        {
            try
            {
                return objdb.Guests.Select(x => new GuestModel
                {
                    GuestId = x.GuestId,
                    EventId = x.EventId,
                    AgentId = x.AgentId,
                    GuestName = x.GuestName,
                    Date = x.Date,
                    AgentName = x.TblUser.Name,
                    NRIC_OR_ID = x.NRIC_OR_ID,
                    Age = x.Age,
                    GuestEmail = x.GuestEmail,
                    Mobile = x.Mobile,
                    EventName = x.Event.EventName,
                }).OrderByDescending(x => x.GuestId).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditGuest(GuestModel objmodel)
        {
            try
            {

                if (objmodel.GuestId == 0)
                {
                    Guest objGuest = new Guest
                    {
                        EventId = objmodel.EventId,
                        AgentId = objmodel.AgentId,
                        GuestName = objmodel.GuestName,
                        NRIC_OR_ID = objmodel.NRIC_OR_ID,
                        Age = objmodel.Age,
                        GuestEmail = objmodel.GuestEmail,
                        Mobile = objmodel.Mobile,
                    };
                    objdb.Guests.Add(objGuest);
                    objdb.SaveChanges();
                    return objGuest.GuestId;
                }
                else
                {
                    var objGuest = objdb.Guests.Find(objmodel.GuestId);
                    objGuest.EventId = objmodel.EventId;
                    objGuest.GuestId = objmodel.GuestId;
                    objGuest.AgentId = objmodel.AgentId;
                    objGuest.GuestName = objmodel.GuestName;
                    objGuest.Age = objmodel.Age;
                    objGuest.Mobile = objmodel.Mobile;
                    objdb.SaveChanges();
                    return objmodel.GuestId;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public GuestModel GetGuestById(int id)
        {
            try
            {
                return objdb.Guests.Where(x => x.GuestId == id).Select(x => new GuestModel
                {
                    GuestId = x.GuestId,
                    EventId = x.EventId,
                    AgentId = x.AgentId,
                    GuestName = x.GuestName,
                    NRIC_OR_ID = x.NRIC_OR_ID,
                    Age = x.Age,
                    GuestEmail = x.GuestEmail,
                    Mobile = x.Mobile,
                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<GuestModel> GetGuestListById(int id)
        {
            try
            {
                return objdb.Guests.Where(x => x.GuestId == id).Select(x => new GuestModel
                {
                    GuestId = x.GuestId,
                    EventId = x.EventId,
                    AgentId = x.AgentId,
                    GuestName = x.GuestName,
                    NRIC_OR_ID = x.NRIC_OR_ID,
                    Age = x.Age,
                    GuestEmail = x.GuestEmail,
                    Mobile = x.Mobile,
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


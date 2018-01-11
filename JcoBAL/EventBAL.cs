using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class EventBAL
    {
        EventDAL objdb = new EventDAL();
        public List<EventModel> AllEvent()
        {
            try
            {
                return objdb.AllEvent();
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
                return objdb.AddEditEvent(objmodel);
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
                return objdb.GetEventById(id);
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
                return objdb.GetEventListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
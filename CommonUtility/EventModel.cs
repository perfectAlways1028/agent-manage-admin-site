using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class EventModel
    {
        public long EventId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public System.DateTime EventDate { get; set; }
        public string EventDateBind { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int AgentSeat { get; set; }
        public bool IsGuestRegistration { get; set; }
        public int GuestSeat { get; set; }
        public System.DateTime RSVP_By { get; set; }
        public string RSVP_By_Bind { get; set; }
        public string RSVP_Time { get; set; }
        public long CreatedBy { get; set; }

        public long CountAgent { get; set; }
        public long CountGuest { get; set; }

        public List<EventModel> EventList { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class GuestModel
    {
        public long GuestId { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; }
        public long AgentId { get; set; }
        public string AgentName { get; set; }
        public string GuestName { get; set; }
        public string NRIC_OR_ID { get; set; }
        public string Age { get; set; }
        public string GuestEmail { get; set; }
        public string Mobile { get; set; }
        public DateTime Date { get; set; }

        public List<GuestModel> GuestList { get; set; }
    
    }
}

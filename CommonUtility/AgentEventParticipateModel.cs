using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class AgentEventParticipateModel
    {
        public long EventparticipateId { get; set; }
        public Nullable<long> EventId { get; set; }
        public string EventName { get; set; }
        public Nullable<long> UserId { get; set; }

        public string AgentName { get; set; }
        public DateTime Date { get; set; }
        public List<AgentEventParticipateModel> AgentEventParticipateList { get; set; }
    }
}

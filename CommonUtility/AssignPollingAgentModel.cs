using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class AssignPollingAgentModel
    {
       public long AssignPollingAgentId { get; set; }
        public long PollId { get; set; }
        public long AgentId { get; set; }

        public List<AssignPollingAgentModel> AssignPollingAgentList { get; set; }
    }
}

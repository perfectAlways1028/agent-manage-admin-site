using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class AssignDutyModel
    {
        public long AssignDutyId { get; set; }
        public long DutyId { get; set; }
        public long AgentId { get; set; }
        public string AgentName { get; set; }

        public List<AssignDutyModel> AssignDutyList { get; set; }
    }
}

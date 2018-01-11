using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class PollResultModel
    {
        public long Id { get; set; }
        public Nullable<long> PollId { get; set; }
        public Nullable<long> AgentId { get; set; }
        public string AgentName { get; set; }
        public string ManagerName { get; set; }
        public Nullable<long> ResultId { get; set; }
        public Nullable<long> Answer { get; set; }
        public string PollTitle { get; set; }
        public string Options { get; set; }
        public string RoleName { get; set; }
        public string Gender { get; set; }
        public string EmailId { get; set; }
        public string Mobile { get; set; }
        public string Code { get; set; }
        public List<PollResultModel> PollResultList { get; set; }
    }
}

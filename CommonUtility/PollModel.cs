using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class PollModel
    {
        public long PollId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime VoteLastDate { get; set; }
        public string VoteLastDateBind { get; set; }
        public bool IsActive { get; set; }
        public long pollResultCount { get; set; }
        public List<PollModel> PollList { get; set; }
        public List<PollOptionModel> PollOptionList { get; set; }
        public List<UserModel> AgentList { get; set; }
    }
}

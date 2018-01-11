using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class PollOptionModel
    {
        public long PollOptionId { get; set; }
        public string Options { get; set; }
        public bool IsTrue { get; set; }
        public long PollId { get; set; }
        public List<PollOptionModel> PollOptionList { get; set; }
    }
}

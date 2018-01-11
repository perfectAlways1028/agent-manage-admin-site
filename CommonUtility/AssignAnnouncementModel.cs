using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class AssignAnnouncementModel
    {
        public long AssignAnnouncementId { get; set; }
        public long AnnouncementId { get; set; }
        public long UserId { get; set; }

        public List<AssignAnnouncementModel> AssignAnnouncementList { get; set; }
    }
}

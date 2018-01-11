using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
  public  class AnnouncementModel
    {
        public long AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public System.DateTime PublishDate { get; set; }
        public System.DateTime ExpireDate { get; set; }

        public string PublishDateBind { get; set; }
        public string ExpireDateBind { get; set; }
        public long CreatedBy { get; set; }
        public List<AnnouncementModel> AnnouncementList { get; set; }
        public List<UserModel> AgentList { get; set; }

    }
}

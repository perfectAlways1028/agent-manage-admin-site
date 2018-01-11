using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CommonUtility
{
    public class AwardModel
    {
        public long AwardId { get; set; }
        public string Title { get; set; }
        public string AwardImage { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public List<AwardModel> AwardList { get; set; }
        public IEnumerable<SelectListItem> ListUser { get; set; }
    }
}

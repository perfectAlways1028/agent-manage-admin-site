using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CommonUtility
{
   public class DutyModel
    {
        public long DutyId { get; set; }
        public string Location { get; set; }
        public System.DateTime DateFrom { get; set; }
        public string DateFromStr { get; set; }
        public string DutyDateBind { get; set; }
        public System.DateTime DateTo { get; set; }
        public string DateToStr { get; set; }
        public string RoadshowCode { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<long> AgentId { get; set; }
        public string AgentName { get; set; }
        public List<DutyModel> DutyList { get; set; }
        public List<UserModel> AgentList { get; set; }
        public List<AssignDutyModel> AssignDutyList { get; set; }
        public IEnumerable<SelectListItem> ListAgent { get; set; }
    }
}

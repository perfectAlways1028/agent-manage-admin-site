using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CommonUtility
{
   public class InceptionModel
    {
        public long InceptionId { get; set; }
        public long MonthFrom { get; set; }

        public long YearFrom { get; set; }

        public long AgentId { get; set; }
        public string RoleName { get; set; }
        public string ManagerName { get; set; }
        public string  AgentName { get; set; }
        public string Code { get; set; }
        public decimal MTD_WAPI { get; set; }
        public decimal YTD_WAPI { get; set; }
        public string Date { get; set; }
        public List<InceptionModel> InceptionList { get; set; }
        public List<DailySubmissionChartModel> DailySubmissionChartList { get; set; }
        public List<UserModel> AgentList { get; set; }
        public IEnumerable<SelectListItem> ListYear { get; set; }
        public IEnumerable<SelectListItem> ListMonth { get; set; }

        public IEnumerable<SelectListItem> ListAgent { get; set; }
    }
}

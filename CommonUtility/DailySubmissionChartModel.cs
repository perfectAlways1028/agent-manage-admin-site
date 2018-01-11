using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
  public  class DailySubmissionChartModel
    {
        public string state { get; set; }
        public Nullable<decimal> Day1 { get; set; }
        public Nullable<decimal> Day { get; set; }
        public Nullable<decimal> Year { get; set; }
        public Nullable<decimal> Month { get; set; }
        public Nullable<decimal> Target { get; set; }
        public Nullable<decimal> Inception { get; set; }
        public string Date { get; set; }
        public List<DailySubmissionChartModel> DailySubmissionChartList { get; set; }
    }
}

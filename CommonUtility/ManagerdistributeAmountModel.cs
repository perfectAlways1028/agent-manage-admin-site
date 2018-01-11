using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
    public class ManagerdistributeAmountModel
    {
        public long ID { get; set; }
        public Nullable<long> AgentId { get; set; }
        public Nullable<long> ManagerId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string AgentName { get; set; }
        public string Manager { get; set; }
        public string Code { get; set; }
        public System.DateTime Date { get; set; }
        public string DateStr { get; set; }
        public bool IsActive { get; set; }
        public long Count { get; set; }
        public List<ManagerdistributeAmountModel> ManagerdistributeAmountList { get; set; }
        public List<ManagerdistributeAmountModel> ManagerdistributeAmountListAdmin { get; set; }
    }
}

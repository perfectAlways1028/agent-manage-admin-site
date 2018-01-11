using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
   public class RoleModel
    {
        public RoleModel()
        {
            Rolemanages = new List<RolemanagementModel>();
        }

        public long RoleId { get; set; }
    
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }


        public int PageID { get; set; }
        public int Total { get; set; }
        public int Current { get; set; }
        public int Pagecount { get; set; }
        public List<RolemanagementModel> Rolemanages { get; set; }
        public List<RoleModel> RolesList { get; set; }
    }

    public class RolemanagementModel
    {
        public long RolemanagementID { get; set; }
        public Nullable<long> RoleID { get; set; }
        public Nullable<long> PageID { get; set; }

        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public string PageName { get; set; }
    }
}

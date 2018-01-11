using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace CommonUtility
{
   public class UserModel
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
        public string Photo { get; set; }
        public string AgentCode { get; set; }
        public string JoinDate { get; set; }
        public string Designation { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public Nullable<decimal> AgentTarget { get; set; }
        public bool IsActive { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string ManagerName { get; set; }
        public Nullable<long> ManagerID { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public List<UserModel> UserList { get; set; }
        public IEnumerable<SelectListItem> ListGender { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        public IEnumerable<SelectListItem> ManagerList { get; set; }
        public long AssignCommonId { get; set; }
        public List<CompanyProfileModel> CompanyProfileList { get; set; }

    }
}

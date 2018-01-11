using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class UserDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<UserModel> AllUser()
        {
            try
            {
                var usr = objdb.TblUsers.ToList();
                return usr.Select(x => new UserModel
                {
                    UserId = x.UserId,
                    EmailId = x.EmailId,

                    Password = x.Password,
                    Mobile = x.Mobile,

                    Name = x.Name,
                    Photo = x.Photo,
                    AgentCode = x.AgentCode,
                    JoinDate = x.JoinDate,
                    ManagerID = x.ManagerID,
                    Designation = x.Designation,
                    Gender = x.Gender,
                    DOB = x.DOB,
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.UpdatedBy,
                    IsActive = x.IsActive,
                    AgentTarget = x.AgentTarget,
                    RoleId = x.RoleId,
                    RoleName = x.Role.RoleName,
                    ManagerName = objdb.TblUsers.Where(y => y.UserId == x.ManagerID).Select(y => y.Name).SingleOrDefault(),

                }).OrderByDescending(x => x.UserId).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditUser(UserModel objmodel)
        {
            try
            {

                if (objmodel.UserId == 0)
                {
                    TblUser objUser = new TblUser
                    {
                        UserId = objmodel.UserId,
                        EmailId = objmodel.EmailId,

                        Password = objmodel.Password,
                        Mobile = objmodel.Mobile,

                        Name = objmodel.Name,
                        Photo = objmodel.Photo,
                        AgentCode = objmodel.AgentCode,
                        JoinDate = objmodel.JoinDate,

                        Designation = objmodel.Designation,
                        Gender = objmodel.Gender,
                        DOB = objmodel.DOB,
                        CreatedBy = objmodel.CreatedBy,
                        UpdatedBy = objmodel.UpdatedBy,
                        IsActive = objmodel.IsActive,
                        RoleId = objmodel.RoleId,
                        ManagerID = objmodel.ManagerID
                    };
                    objdb.TblUsers.Add(objUser);
                    objdb.SaveChanges();
                    return objUser.UserId;
                }
                else
                {
                    var objUser = objdb.TblUsers.Find(objmodel.UserId);
                    objUser.UserId = objmodel.UserId;
                    objUser.EmailId = objmodel.EmailId;
                    objUser.Password = objmodel.Password;
                    objUser.Mobile = objmodel.Mobile;
                    objUser.Name = objmodel.Name;
                    objUser.Designation = objmodel.Designation;
                    objUser.AgentCode = objmodel.AgentCode;
                    objUser.JoinDate = objmodel.JoinDate;
                    if (objmodel.Photo != null && objmodel.Photo != "")
                    {
                        objUser.Photo = objmodel.Photo;
                       
                    }
                    objUser.DOB = objmodel.DOB;
                    objUser.IsActive = objmodel.IsActive;
                    objUser.RoleId = objmodel.RoleId;
                    objUser.ManagerID = objmodel.ManagerID;
                    //objUser.CreatedBy = objmodel.CreatedBy;

                    objUser.UpdatedBy = objmodel.UpdatedBy;
                    objdb.SaveChanges();
                    return objmodel.UserId;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public UserModel GetUserById(int id)
        {
            try
            {
                return objdb.TblUsers.Where(x => x.UserId == id).Select(x => new UserModel
                {
                    UserId = x.UserId,
                    EmailId = x.EmailId,

                    Password = x.Password,
                    Mobile = x.Mobile,

                    Name = x.Name,
                    Photo = x.Photo,
                    AgentCode = x.AgentCode,
                    JoinDate = x.JoinDate,

                    Designation = x.Designation,
                    Gender = x.Gender,

                    UpdatedBy = x.UpdatedBy,
                    ManagerID=x.ManagerID,
                    CreatedBy = x.CreatedBy,
                    DOB = x.DOB,
                    IsActive = x.IsActive,
                    RoleId = x.RoleId,
                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public List<UserModel> GetUserListById(int id)
        {
            try
            {
                return objdb.TblUsers.Where(x => x.UserId == id).Select(x => new UserModel
                {
                    UserId = x.UserId,
                    EmailId = x.EmailId,

                    Password = x.Password,
                    Mobile = x.Mobile,

                    Name = x.Name,
                    Photo = x.Photo,
                    AgentCode = x.AgentCode,
                    JoinDate = x.JoinDate,

                    Designation = x.Designation,
                    Gender = x.Gender,
                    ManagerID = x.ManagerID,
                    UpdatedBy = x.UpdatedBy,
                    CreatedBy = x.CreatedBy,
                    DOB = x.DOB,
                    IsActive = x.IsActive,
                    RoleId = x.RoleId,
                }).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public UserModel Login(string EmailId, string pass)
        {
            try
            {
                return objdb.TblUsers.Where(x => x.EmailId == EmailId && x.Password == pass && x.RoleId!=3).Select(x => new UserModel
                {
                    UserId = x.UserId,
                    EmailId = x.EmailId,
                    Password = x.Password,
                    Mobile = x.Mobile,

                    Name = x.Name,
                    Photo = x.Photo,
                    AgentCode = x.AgentCode,
                    JoinDate = x.JoinDate,

                    Designation = x.Designation,
                    Gender = x.Gender,
                    UpdatedBy = x.UpdatedBy,
                    CreatedBy = x.CreatedBy,
                    DOB = x.DOB,
                    IsActive = x.IsActive,

                    RoleId = x.RoleId,
                }).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public bool CheckDuplicateEmailId(string EmailIdId)
        {
            bool res = false;

            try
            {
                var obj = objdb.TblUsers.Where(x => x.EmailId == EmailIdId).FirstOrDefault();

                if (obj != null)
                {
                    res = true;
                }
                return res;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool CheckDuplicateAgentCode(string code)
        {
            bool res = false;

            try
            {
                var obj = objdb.TblUsers.Where(x => x.AgentCode == code).FirstOrDefault();

                if (obj != null)
                {
                    res = true;
                }
                return res;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool CheckDuplicateMob(string Mobile)
        {
            bool res = false;

            try
            {
                var obj = objdb.TblUsers.Where(x => x.Mobile == Mobile).FirstOrDefault();

                if (obj != null)
                {
                    res = true;
                }
                return res;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class UserBAL
    {
        UserDAL objdb = new UserDAL();
        public List<UserModel> AllUser()
        {
            try
            {
                return objdb.AllUser();
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
                return objdb.AddEditUser(objmodel);
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
                return objdb.GetUserById(id);
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
                return objdb.GetUserListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public UserModel Login(string email, string pass)
        {
            try
            {
                return objdb.Login(email, pass);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public bool CheckDuplicateEmail(string emailId)
        {
            bool res = false;

            try
            {
                return objdb.CheckDuplicateEmailId(emailId);
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
                return objdb.CheckDuplicateAgentCode(code);
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
                return objdb.CheckDuplicateMob(Mobile);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}

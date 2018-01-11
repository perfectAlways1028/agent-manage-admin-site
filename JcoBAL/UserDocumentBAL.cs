using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class UserDocumentBAL
    {
        UserDocumentDAL objdb = new UserDocumentDAL();
        public List<UserDocumentModel> AllUserDocument()
        {
            try
            {
                return objdb.AllUserDocument();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public long AddEditUserDocument(UserDocumentModel objmodel)
        {
            try
            {
                return objdb.AddEditUserDocument(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public UserDocumentModel GetUserDocumentById(int id)
        {
            try
            {
                return objdb.GetUserDocumentById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<UserDocumentModel> GetUserDocumentListById(int id)
        {
            try
            {
                return objdb.GetUserDocumentListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public bool ChangeDocumentStatus(long id)
        {
          
                return objdb.ChangeDocumentStatus(id);
         
       
        }
    }
}
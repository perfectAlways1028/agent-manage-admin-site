using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class SubjectBAL
    {
        SubjectDAL objdb = new SubjectDAL();
        public List<SubjectModel> AllSubject()
        {
            try
            {
                return objdb.AllSubject();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public long AddEditSubject(SubjectModel objmodel)
        {
            try
            {
                return objdb.AddEditSubject(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public SubjectModel GetSubjectById(int id)
        {
            try
            {
                return objdb.GetSubjectById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<SubjectModel> GetSubjectListById(int id)
        {
            try
            {
                return objdb.GetSubjectListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
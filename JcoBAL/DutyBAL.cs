using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class DutyBAL
    {
        DutyDAL objdb = new DutyDAL();
        public List<DutyModel> AllDuty()
        {
            try
            {
                return objdb.AllDuty();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public long AddEditDuty(DutyModel objmodel)
        {
            try
            {
                return objdb.AddEditDuty(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public DutyModel GetDutyById(int id)
        {
            try
            {
                return objdb.GetDutyById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<DutyModel> GetDutyListById(int id)
        {
            try
            {
                return objdb.GetDutyListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
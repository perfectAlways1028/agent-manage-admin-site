using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
  public class AwardBAL
    {
        AwardDAL objdb = new AwardDAL();
        public List<AwardModel> AllAward()
        {
            try
            {
                return objdb.AllAward();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public long AddEditAward(AwardModel objmodel)
        {
            try
            {
                return objdb.AddEditAward(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public AwardModel GetAwardById(int id)
        {
            try
            {
                return objdb.GetAwardById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<AwardModel> GetAwardListById(int id)
        {
            try
            {
                return objdb.GetAwardListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
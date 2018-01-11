using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
    public class PollOptionBAL
    {
        PollOptionDAL objdb = new PollOptionDAL();
        public List<PollOptionModel> AllPollOption()
        {
            try
            {
                return objdb.AllPollOption();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public long AddEditPollOption(PollOptionModel objmodel)
        {
            try
            {
                return objdb.AddEditPollOption(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public PollOptionModel GetPollOptionById(int id)
        {
            try
            {
                return objdb.GetPollOptionById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<PollOptionModel> GetPollOptionListById(int id)
        {
            try
            {
                return objdb.GetPollOptionListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public bool ChangePollStatus(int id)
        {
            try
            {
                return objdb.ChangePollStatus(id);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
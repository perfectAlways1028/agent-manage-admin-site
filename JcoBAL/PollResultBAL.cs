using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
    public class PollResultBAL
    {
        PollResultDAL objdb = new PollResultDAL();
        public List<PollResultModel> AllPoll()
        {
            try
            {
                return objdb.AllPollResult();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public PollResultModel GetPollResultById(long PollId)
        {
            try
            {
                return objdb.GetPollResultById(PollId);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class PollResultDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<PollResultModel> AllPollResult()
        {
            try
            {
                var sd=objdb.PollResults.ToList();
                return sd.Select(x => new PollResultModel
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    PollId = x.PollId,
                    AgentName = x.TblUser.Name,
                    ResultId = x.ResultId,
                    Answer = objdb.PollOptions.Where(y => y.PollId == x.PollId && y.IsTrue == true).Select(y => y.PollOptionId).SingleOrDefault(),
                    Options = objdb.PollOptions.Where(y => y.PollId == x.PollId && y.IsTrue == true).Select(y => y.Options).SingleOrDefault(),

                }).OrderByDescending(x => x.PollId).ToList();
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
                return objdb.PollResults.Where(x => x.PollId == PollId).Select(x => new PollResultModel
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    PollId = x.PollId,
                    ResultId = x.ResultId,
                    AgentName = x.TblUser.Name,

                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class PollDAL
   {
       JCOEntities objdb = new JCOEntities();
       public List<PollModel> AllPoll()
       {
           try
           {
               var list = objdb.Polls.Select(x => new 
               {
                   PollId = x.PollId,
                   Title = x.Title,
                   Description = x.Description,
                   CreatedDate = x.CreatedDate,
                   CreatedBy = x.CreatedBy,
                   VoteLastDate = x.VoteLastDate,
                   Image = x.Image,
                   pollResultCount = objdb.PollResults.Where(y=>y.PollId==x.PollId).Count(),
                   IsActive = x.IsActive,
               }).OrderByDescending(x => x.PollId).ToList();

               return list.Select(x => new PollModel
               {
                   PollId = x.PollId,
                   Title = x.Title,
                   Description = x.Description,
                   CreatedDate = x.CreatedDate,
                   CreatedBy = x.CreatedBy,
                   VoteLastDate = x.VoteLastDate,
                   Image = x.Image,
                   pollResultCount = x.pollResultCount,
                   IsActive = x.IsActive,
               }).OrderByDescending(x => x.PollId).ToList();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }
       public long AddEditPoll(PollModel objmodel)
       {
           try
           {

               if (objmodel.PollId == 0)
               {
                   Poll objPoll = new Poll
                   {
                       Title = objmodel.Title,
                       Description = objmodel.Description,
                       CreatedDate = objmodel.CreatedDate,
                       CreatedBy = objmodel.CreatedBy,
                       VoteLastDate = objmodel.VoteLastDate,
                       Image = objmodel.Image,
                       IsActive = objmodel.IsActive,

                   };
                   objdb.Polls.Add(objPoll);
                   objdb.SaveChanges();
                   return objPoll.PollId;
               }
               else
               {
                   var objPoll = objdb.Polls.Find(objmodel.PollId);
                   objPoll.Title = objmodel.Title;
                   objPoll.PollId = objmodel.PollId;
                   objPoll.Description = objmodel.Description;
                   if (objmodel.Image != null && objmodel.Image != "")
                   {
                       objPoll.Image = objmodel.Image;
                   }
                   objPoll.VoteLastDate = objmodel.VoteLastDate;
                   objPoll.IsActive = objmodel.IsActive;
                   objdb.SaveChanges();
                   return objmodel.PollId;
               }
           }
           catch (Exception)
           {
               return 0;
               throw;
           }
       }

       public PollModel GetPollById(int id)
       {
           try
           {
               return objdb.Polls.Where(x => x.PollId == id).Select(x => new PollModel
               {
                   PollId = x.PollId,
                   Title = x.Title,
                   Description = x.Description,
                   CreatedDate = x.CreatedDate,
                   CreatedBy = x.CreatedBy,
                   VoteLastDate = x.VoteLastDate,
                   Image = x.Image,
                   IsActive = x.IsActive,
               }).SingleOrDefault();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }

       public List<PollModel> GetPollListById(int id)
       {
           try
           {
               return objdb.Polls.Where(x => x.PollId == id).Select(x => new PollModel
               {
                   PollId = x.PollId,
                   Title = x.Title,
                   Description = x.Description,
                   CreatedDate = x.CreatedDate,
                   CreatedBy = x.CreatedBy,
                   Image = x.Image,
                   VoteLastDate = x.VoteLastDate,
                   IsActive = x.IsActive,
             
               }).ToList();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


   }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;


namespace JcoDAL
{
   public class PollOptionDAL
   {
       JCOEntities objdb = new JCOEntities();
       public List<PollOptionModel> AllPollOption()
       {
           try
           {
               return objdb.PollOptions.Select(x => new PollOptionModel
               {
                   PollOptionId = x.PollOptionId,
                   Options = x.Options,
                   IsTrue = x.IsTrue,
                   PollId = x.PollId,
               }).OrderByDescending(x => x.PollOptionId).ToList();
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

               if (objmodel.PollOptionId == 0)
               {
                   PollOption objPollOption = new PollOption
                   {
                       Options = objmodel.Options,
                       IsTrue = objmodel.IsTrue,
                       PollId = objmodel.PollId,
                   };
                   objdb.PollOptions.Add(objPollOption);
                   objdb.SaveChanges();
                   return objPollOption.PollOptionId;
               }
               else
               {
                   var objPollOption = objdb.PollOptions.Find(objmodel.PollOptionId);
                   objPollOption.Options = objmodel.Options;
                   objPollOption.PollOptionId = objmodel.PollOptionId;
                   objPollOption.IsTrue = objmodel.IsTrue;
                   objPollOption.PollId = objmodel.PollId;
                   objdb.SaveChanges();
                   return objmodel.PollOptionId;
               }
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
               return objdb.PollOptions.Where(x => x.PollOptionId == id).Select(x => new PollOptionModel
               {
                   PollOptionId = x.PollOptionId,
                   Options = x.Options,
                   IsTrue = x.IsTrue,
                   PollId = x.PollId,

               }).SingleOrDefault();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }

       public List<PollOptionModel> GetPollOptionListById(int PollId)
       {
           try
           {
               return objdb.PollOptions.Where(x => x.PollId == PollId).Select(x => new PollOptionModel
               {
                   PollOptionId = x.PollOptionId,
                   Options = x.Options,
                   IsTrue = x.IsTrue,
                   PollId = x.PollId,

               }).ToList();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }

       public bool ChangePollStatus(long id)
       {
           try
           {
               var obj = objdb.Polls.Find(id);
               if (obj != null && obj.IsActive == true)
               {
                   obj.IsActive = false;
                   objdb.SaveChanges();
                   return false;
               }
               else if (obj != null)
               {
                   obj.IsActive = true;
                   objdb.SaveChanges();
                   return true;
               }
               return false;
           }
           catch (Exception)
           {
               return false;
               throw;
           }

       }
   }
}


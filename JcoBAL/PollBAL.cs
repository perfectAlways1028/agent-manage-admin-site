using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
  public  class PollBAL
  {
      PollDAL objdb = new PollDAL();
      public List<PollModel> AllPoll()
      {
          try
          {
              return objdb.AllPoll();
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
              return objdb.AddEditPoll(objmodel);
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
              return objdb.GetPollById(id);
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
              return objdb.GetPollListById(id);
          }
          catch (Exception)
          {
              return null;
              throw;
          }
      }
  }
}
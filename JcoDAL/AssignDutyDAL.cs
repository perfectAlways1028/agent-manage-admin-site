using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;


namespace JcoDAL
{
   public class AssignDutyDAL
   {
       JCOEntities objdb = new JCOEntities();
       public List<AssignDutyModel> AllAssignDuty()
       {
           try
           {
               return objdb.AssignDuties.Select(x => new AssignDutyModel
               {
                   AssignDutyId = x.AssignDutyId,
                   AgentId = x.AgentId,
                   DutyId = x.DutyId,

               }).OrderByDescending(x => x.AssignDutyId).ToList();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }
       public long AddEditAssignDuty(AssignDutyModel objmodel)
       {
           try
           {

               if (objmodel.AssignDutyId == 0)
               {
                   AssignDuty objAssignDuty = new AssignDuty
                   {
                       AgentId = objmodel.AgentId,
                       DutyId = objmodel.DutyId,
                       UserAvailable = true,
      

                   };
                   objdb.AssignDuties.Add(objAssignDuty);
                   objdb.SaveChanges();
                   return objAssignDuty.AssignDutyId;
               }
               else
               {
                   var objAssignDuty = objdb.AssignDuties.Find(objmodel.AssignDutyId);
                   objAssignDuty.AgentId = objmodel.AgentId;
                   objAssignDuty.AssignDutyId = objmodel.AssignDutyId;
                   objAssignDuty.DutyId = objmodel.DutyId;
         

                   objdb.SaveChanges();
                   return objmodel.AssignDutyId;
               }
           }
           catch (Exception)
           {
               return 0;
               throw;
           }
       }

       public AssignDutyModel GetAssignDutyById(int id)
       {
           try
           {
               return objdb.AssignDuties.Where(x => x.AssignDutyId == id).Select(x => new AssignDutyModel
               {
                   AssignDutyId = x.AssignDutyId,
                   AgentId = x.AgentId,
                   DutyId = x.DutyId,

                   //AgentId = x.AgentId,
               }).SingleOrDefault();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }

       public List<AssignDutyModel> GetAssignDutyListById(int id)
       {
           try
           {
               return objdb.AssignDuties.Where(x => x.DutyId == id).Select(x => new AssignDutyModel
               {
                   AssignDutyId = x.AssignDutyId,
                   AgentId = x.AgentId,
                   DutyId = x.DutyId,
                   AgentName = x.TblUser.Name,
        
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



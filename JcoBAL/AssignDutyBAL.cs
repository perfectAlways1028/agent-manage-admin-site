using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class AssignDutyBAL
   {
       AssignDutyDAL objdb = new AssignDutyDAL();
       public List<AssignDutyModel> AllAssignDuty()
       {
           try
           {
               return objdb.AllAssignDuty();
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
               return objdb.AddEditAssignDuty(objmodel);
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
               return objdb.GetAssignDutyById(id);
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
               return objdb.GetAssignDutyListById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }
   }
}
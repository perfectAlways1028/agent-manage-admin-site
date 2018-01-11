using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class AssignPollingAgentBAL
   {
       AssignPollingAgentDAL objdb = new AssignPollingAgentDAL();
       public List<AssignPollingAgentModel> AllAssignPollingAgent()
       {
           try
           {
               return objdb.AllAssignPollingAgent();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


       public long AddEditAssignPollingAgent(AssignPollingAgentModel objmodel)
       {
           try
           {
               return objdb.AddEditAssignPollingAgent(objmodel);
           }
           catch (Exception)
           {
               return 0;
               throw;
           }
       }

       public AssignPollingAgentModel GetAssignPollingAgentById(int id)
       {
           try
           {
               return objdb.GetAssignPollingAgentById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


       public List<AssignPollingAgentModel> GetAssignPollingAgentListById(int id)
       {
           try
           {
               return objdb.GetAssignPollingAgentListById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


   }
}
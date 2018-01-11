using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;


namespace JcoDAL
{
   public class AssignPollingAgentDAL
   {
       JCOEntities objdb = new JCOEntities();
       public List<AssignPollingAgentModel> AllAssignPollingAgent()
       {
           try
           {
               return objdb.AssignPollingAgents.Select(x => new AssignPollingAgentModel
               {
                   AssignPollingAgentId = x.AssignPollingAgentId,
                   AgentId = x.AgentId,
                   PollId = x.PollId,

               }).OrderByDescending(x => x.AssignPollingAgentId).ToList();
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

               if (objmodel.AssignPollingAgentId == 0)
               {
                   AssignPollingAgent objAssignPollingAgent = new AssignPollingAgent
                   {
                       AgentId = objmodel.AgentId,
                       PollId = objmodel.PollId,
                   };
                   objdb.AssignPollingAgents.Add(objAssignPollingAgent);
                   objdb.SaveChanges();
                   return objAssignPollingAgent.AssignPollingAgentId;
               }
               else
               {
                   var objAssignPollingAgent = objdb.AssignPollingAgents.Find(objmodel.AssignPollingAgentId);
                   objAssignPollingAgent.AgentId = objmodel.AgentId;
                   objAssignPollingAgent.AssignPollingAgentId = objmodel.AssignPollingAgentId;
                   objAssignPollingAgent.PollId = objmodel.PollId;

                   objdb.SaveChanges();
                   return objmodel.AssignPollingAgentId;
               }
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
               return objdb.AssignPollingAgents.Where(x => x.AssignPollingAgentId == id).Select(x => new AssignPollingAgentModel
               {
                   AssignPollingAgentId = x.AssignPollingAgentId,
                   AgentId = x.AgentId,
                   PollId = x.PollId,

               }).SingleOrDefault();
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
               return objdb.AssignPollingAgents.Where(x => x.AssignPollingAgentId == id).Select(x => new AssignPollingAgentModel
               {
                   AssignPollingAgentId = x.AssignPollingAgentId,
                   AgentId = x.AgentId,
                   PollId = x.PollId,
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

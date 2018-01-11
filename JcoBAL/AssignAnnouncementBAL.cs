using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class AssignAnnouncementBAL
   {
       AssignAnnouncementDAL objdb = new AssignAnnouncementDAL();
       public List<AssignAnnouncementModel> AllAssignAnnouncement()
       {
           try
           {
               return objdb.AllAssignAnnouncement();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


       public long AddEditAssignAnnouncement(AssignAnnouncementModel objmodel)
       {
           try
           {
               return objdb.AddEditAssignAnnouncement(objmodel);
           }
           catch (Exception)
           {
               return 0;
               throw;
           }
       }

       public AssignAnnouncementModel GetAssignAnnouncementById(int id)
       {
           try
           {
               return objdb.GetAssignAnnouncementById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


       public List<AssignAnnouncementModel> GetAssignAnnouncementListById(int id)
       {
           try
           {
               return objdb.GetAssignAnnouncementListById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


   }
}
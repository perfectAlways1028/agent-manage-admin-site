using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class AnnouncementBAL
    {
        AnnouncementDAL objdb = new AnnouncementDAL();
        public List<AnnouncementModel> AllAnnouncement()
        {
            try
            {
                return objdb.AllAnnouncement();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public long AddEditAnnouncement(AnnouncementModel objmodel)
        {
            try
            {
                return objdb.AddEditAnnouncement(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public AnnouncementModel GetAnnouncementById(int id)
        {
            try
            {
                return objdb.GetAnnouncementById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<AnnouncementModel> GetAnnouncementListById(int id)
        {
            try
            {
                return objdb.GetAnnouncementListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


    }
}
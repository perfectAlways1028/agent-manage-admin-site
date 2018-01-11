using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class AnnouncementDAL
    {
       JCOEntities objdb = new JCOEntities();
        public List<AnnouncementModel> AllAnnouncement()
        {
            try
            {
                return objdb.Announcements.Select(x => new AnnouncementModel
                {
                    AnnouncementId = x.AnnouncementId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    PublishDate = x.PublishDate,
                    ExpireDate = x.ExpireDate,
                    CreatedBy = x.CreatedBy,
                }).OrderByDescending(x => x.AnnouncementId).ToList();
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

                if (objmodel.AnnouncementId == 0)
                {
                    Announcement objAnnouncement = new Announcement
                    {
                        Title = objmodel.Title,
                        Description = objmodel.Description,
                        Image = objmodel.Image,
                        PublishDate = objmodel.PublishDate,
                        ExpireDate = objmodel.ExpireDate,
                        CreatedBy = objmodel.CreatedBy,
                    };
                    objdb.Announcements.Add(objAnnouncement);
                    objdb.SaveChanges();
                    return objAnnouncement.AnnouncementId;
                }
                else
                {
                    var objAnnouncement = objdb.Announcements.Find(objmodel.AnnouncementId);
                    objAnnouncement.Title = objmodel.Title;
                    objAnnouncement.AnnouncementId = objmodel.AnnouncementId;
                    objAnnouncement.PublishDate = objmodel.PublishDate;
                    objAnnouncement.Description = objmodel.Description;
                    if (objmodel.Image != null && objmodel.Image != "")
                    {
                        objAnnouncement.Image = objmodel.Image;
                    }
                
                    objAnnouncement.ExpireDate = objmodel.ExpireDate;
                    objdb.SaveChanges();
                    return objmodel.AnnouncementId;
                }
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
                return objdb.Announcements.Where(x => x.AnnouncementId == id).Select(x => new AnnouncementModel
                {
                    AnnouncementId = x.AnnouncementId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    PublishDate = x.PublishDate,
                    ExpireDate = x.ExpireDate,
                    CreatedBy = x.CreatedBy,
                }).SingleOrDefault();
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
                return objdb.Announcements.Where(x => x.AnnouncementId == id).Select(x => new AnnouncementModel
                {
                    AnnouncementId = x.AnnouncementId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    PublishDate = x.PublishDate,
                    ExpireDate = x.ExpireDate,
                    CreatedBy = x.CreatedBy,
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
  public  class AssignAnnouncementDAL
  {
      JCOEntities objdb = new JCOEntities();
      public List<AssignAnnouncementModel> AllAssignAnnouncement()
      {
          try
          {
              return objdb.AssignAnnouncements.Select(x => new AssignAnnouncementModel
              {
                  AssignAnnouncementId = x.AssignAnnouncementId,
                  UserId = x.UserId,
                  AnnouncementId = x.AnnouncementId,

              }).OrderByDescending(x => x.AssignAnnouncementId).ToList();
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

              if (objmodel.AssignAnnouncementId == 0)
              {
                  AssignAnnouncement objAssignAnnouncement = new AssignAnnouncement
                  {
                      UserId = objmodel.UserId,
                      AnnouncementId = objmodel.AnnouncementId,
                  };
                  objdb.AssignAnnouncements.Add(objAssignAnnouncement);
                  objdb.SaveChanges();
                  return objAssignAnnouncement.AssignAnnouncementId;
              }
              else
              {
                  var objAssignAnnouncement = objdb.AssignAnnouncements.Find(objmodel.AssignAnnouncementId);
                  objAssignAnnouncement.UserId = objmodel.UserId;
                  objAssignAnnouncement.AssignAnnouncementId = objmodel.AssignAnnouncementId;
                  objAssignAnnouncement.AnnouncementId = objmodel.AnnouncementId;

                  objdb.SaveChanges();
                  return objmodel.AssignAnnouncementId;
              }
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
              return objdb.AssignAnnouncements.Where(x => x.AssignAnnouncementId == id).Select(x => new AssignAnnouncementModel
              {
                  AssignAnnouncementId = x.AssignAnnouncementId,
                  UserId = x.UserId,
                  AnnouncementId = x.AnnouncementId,

              }).SingleOrDefault();
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
              return objdb.AssignAnnouncements.Where(x => x.AssignAnnouncementId == id).Select(x => new AssignAnnouncementModel
              {
                  AssignAnnouncementId = x.AssignAnnouncementId,
                  UserId = x.UserId,
                  AnnouncementId = x.AnnouncementId,
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

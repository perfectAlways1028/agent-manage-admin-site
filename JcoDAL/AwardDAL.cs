using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class AwardDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<AwardModel> AllAward()
        {
            try
            {
                return objdb.Awards.Select(x => new AwardModel
                {
                    AwardId = x.AwardId,
                    Title = x.Title,
                    UserId = x.UserId,
                    UserName=x.TblUser.Name,
                    AwardImage = x.AwardImage,

                }).OrderByDescending(x => x.AwardId).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditAward(AwardModel objmodel)
        {
            try
            {

                if (objmodel.AwardId == 0)
                {
                    Award objAward = new Award
                    {
                        Title = objmodel.Title,
                        UserId = objmodel.UserId,
                        AwardImage = objmodel.AwardImage,

                    };
                    objdb.Awards.Add(objAward);
                    objdb.SaveChanges();
                    return objAward.AwardId;
                }
                else
                {
                    var objAward = objdb.Awards.Find(objmodel.AwardId);
                    objAward.Title = objmodel.Title;
                    objAward.AwardId = objmodel.AwardId;
                    objAward.UserId = objmodel.UserId;
                    if (objmodel.AwardImage != null && objmodel.AwardImage != "")
                    {
                        objAward.AwardImage = objmodel.AwardImage;
                    }
                    objdb.SaveChanges();
                    return objmodel.AwardId;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public AwardModel GetAwardById(int id)
        {
            try
            {
                return objdb.Awards.Where(x => x.AwardId == id).Select(x => new AwardModel
                {
                    AwardId = x.AwardId,
                    Title = x.Title,
                    UserId = x.UserId,
                    AwardImage = x.AwardImage,

                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<AwardModel> GetAwardListById(int id)
        {
            try
            {
                return objdb.Awards.Where(x => x.AwardId == id).Select(x => new AwardModel
                {
                    AwardId = x.AwardId,
                    Title = x.Title,
                    UserId = x.UserId,
                    AwardImage = x.AwardImage,

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

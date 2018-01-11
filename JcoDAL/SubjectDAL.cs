using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class SubjectDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<SubjectModel> AllSubject()
        {
            try
            {
                return objdb.Subjects.Select(x => new SubjectModel
                {
                    SubjectId = x.SubjectId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    CreatedBy = x.CreatedBy,
                    IsActive = x.IsActive,
                }).OrderByDescending(x => x.SubjectId).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditSubject(SubjectModel objmodel)
        {
            try
            {

                if (objmodel.SubjectId == 0)
                {
                    Subject objSubject = new Subject
                    {
                        Title = objmodel.Title,
                        Description = objmodel.Description,
                        Image = objmodel.Image,
                        CreatedBy = objmodel.CreatedBy,
                        IsActive = objmodel.IsActive,
                    };
                    objdb.Subjects.Add(objSubject);
                    objdb.SaveChanges();
                    return objSubject.SubjectId;
                }
                else
                {
                    var objSubject = objdb.Subjects.Find(objmodel.SubjectId);
                    objSubject.Title = objmodel.Title;
                    objSubject.SubjectId = objmodel.SubjectId;
                    objSubject.Description = objmodel.Description;
                    objSubject.Image = objmodel.Image;
                    objSubject.IsActive = objmodel.IsActive;
                    objdb.SaveChanges();
                    return objmodel.SubjectId;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public SubjectModel GetSubjectById(int id)
        {
            try
            {
                return objdb.Subjects.Where(x => x.SubjectId == id).Select(x => new SubjectModel
                {
                    SubjectId = x.SubjectId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    CreatedBy = x.CreatedBy,
                    IsActive = x.IsActive,
                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<SubjectModel> GetSubjectListById(int id)
        {
            try
            {
                return objdb.Subjects.Where(x => x.SubjectId == id).Select(x => new SubjectModel
                {
                    SubjectId = x.SubjectId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    CreatedBy = x.CreatedBy,
                    IsActive = x.IsActive,
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


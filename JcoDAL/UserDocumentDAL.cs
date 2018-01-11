using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;


namespace JcoDAL
{
   public class UserDocumentDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<UserDocumentModel> AllUserDocument()
        {
            try
            {
                return objdb.UserDocuments.Select(x => new UserDocumentModel
                {
                    UserDocumentId = x.UserDocumentId,
                    DocumentName = x.DocumentName,
                    Title = x.Title,
                    Description=x.Description,
                    DocType=x.DocType,
                    UserId = x.UserId,
                    UploaderName=x.TblUser.Name,
                    DocumentExtension = x.DocumentExtension,
                    IsActive=x.IsActive,
                }).OrderByDescending(x => x.UserDocumentId).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditUserDocument(UserDocumentModel objmodel)
        {
            try
            {

                if (objmodel.UserDocumentId == 0)
                {
                    UserDocument objUserDocument = new UserDocument
                    {
                        DocumentName = objmodel.DocumentName,
                        Title = objmodel.Title,
                        Description=objmodel.Description,
                        DocType = objmodel.DocType,
                        UserId = objmodel.UserId,
                        DocumentExtension = objmodel.DocumentExtension,
                        IsActive =objmodel.IsActive,

                    };
                    objdb.UserDocuments.Add(objUserDocument);
                    objdb.SaveChanges();
                    return objUserDocument.UserDocumentId;
                }
                else
                {
                    var objUserDocument = objdb.UserDocuments.Find(objmodel.UserDocumentId);
                    if (objmodel.DocumentName != null && objmodel.DocumentName != "")
                    {
                        objUserDocument.DocumentName = objmodel.DocumentName;
                    }
            
                    objUserDocument.Title = objmodel.Title;
                    objUserDocument.DocType = objmodel.DocType;
                    objUserDocument.UserDocumentId = objmodel.UserDocumentId;
                    objUserDocument.UserId = objmodel.UserId;
                    objUserDocument.DocumentExtension = objmodel.DocumentExtension;
                    objUserDocument.IsActive = objmodel.IsActive;

                    objdb.SaveChanges();
                    return objmodel.UserDocumentId;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public UserDocumentModel GetUserDocumentById(int id)
        {
            try
            {
                return objdb.UserDocuments.Where(x => x.UserDocumentId == id).Select(x => new UserDocumentModel
                {
                    UserDocumentId = x.UserDocumentId,
                    DocumentName = x.DocumentName,
                    Title = x.Title,
                    Description = x.Description,
                    DocType = x.DocType,
                    UserId = x.UserId,
                    DocumentExtension = x.DocumentExtension,
                    IsActive = x.IsActive,
   
                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<UserDocumentModel> GetUserDocumentListById(int id)
        {
            try
            {
                return objdb.UserDocuments.Where(x => x.UserDocumentId == id).Select(x => new UserDocumentModel
                {
                    UserDocumentId = x.UserDocumentId,
                    DocumentName = x.DocumentName,
                    Title = x.Title,
                    Description = x.Description,
                    DocType = x.DocType,
                    UserId = x.UserId,
                    DocumentExtension = x.DocumentExtension,
                    IsActive = x.IsActive,

                }).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public bool ChangeDocumentStatus(long id)
        {
            try
            {
                var obj = objdb.UserDocuments.Find(id);
                if (obj != null && obj.IsActive == true)
                {
                    obj.IsActive = false;
                    objdb.SaveChanges();
                    return false;
                }
                else if (obj != null)
                {
                    obj.IsActive = true;
                    objdb.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

    }
}


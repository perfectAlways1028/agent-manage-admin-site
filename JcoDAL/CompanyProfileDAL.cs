using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class CompanyProfileDAL
   {
       JCOEntities objdb = new JCOEntities();
       public List<CompanyProfileModel> AllCompanyProfile()
       {
           try
           {
               return objdb.CompanyProfiles.Select(x => new CompanyProfileModel
               {
                   ProfileId = x.ProfileId,
                   Title = x.Title,
                   Description = x.Description,
                   Logo = x.Logo,

               }).OrderByDescending(x => x.ProfileId).ToList();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }
       public long AddEditCompanyProfile(CompanyProfileModel objmodel)
       {
           try
           {

               if (objmodel.ProfileId == 0)
               {
                   CompanyProfile objCompanyProfile = new CompanyProfile
                   {
                       Title = objmodel.Title,
                       Description = objmodel.Description,
                       Logo = objmodel.Logo,

                   };
                   objdb.CompanyProfiles.Add(objCompanyProfile);
                   objdb.SaveChanges();
                   return objCompanyProfile.ProfileId;
               }
               else
               {
                   var objCompanyProfile = objdb.CompanyProfiles.Find(objmodel.ProfileId);
                   objCompanyProfile.Title = objmodel.Title;
                   objCompanyProfile.ProfileId = objmodel.ProfileId;
                   objCompanyProfile.Description = objmodel.Description;
                   if (objmodel.Logo != null && objmodel.Logo != "")
                   {
                       objCompanyProfile.Logo = objmodel.Logo;
                   }


                   objdb.SaveChanges();
                   return objmodel.ProfileId;
               }
           }
           catch (Exception)
           {
               return 0;
               throw;
           }
       }

       public CompanyProfileModel GetCompanyProfileById(int id)
       {
           try
           {
               return objdb.CompanyProfiles.Where(x => x.ProfileId == id).Select(x => new CompanyProfileModel
               {
                   ProfileId = x.ProfileId,
                   Title = x.Title,
                   Description = x.Description,
                   Logo = x.Logo,

               }).SingleOrDefault();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }

       public List<CompanyProfileModel> GetCompanyProfileListById(int id)
       {
           try
           {
               return objdb.CompanyProfiles.Where(x => x.ProfileId == id).Select(x => new CompanyProfileModel
               {
                   ProfileId = x.ProfileId,
                   Title = x.Title,
                   Description = x.Description,
                   Logo = x.Logo,

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



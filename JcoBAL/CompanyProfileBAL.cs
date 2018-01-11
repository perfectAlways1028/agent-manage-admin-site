using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class CompanyProfileBAL
   {
       CompanyProfileDAL objdb = new CompanyProfileDAL();
       public List<CompanyProfileModel> AllCompanyProfile()
       {
           try
           {
               return objdb.AllCompanyProfile();
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
               return objdb.AddEditCompanyProfile(objmodel);
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
               return objdb.GetCompanyProfileById(id);
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
               return objdb.GetCompanyProfileListById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }
   }
}
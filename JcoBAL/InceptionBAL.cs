using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class InceptionBAL
   {
       InceptionDAL objdb = new InceptionDAL();
       public List<InceptionModel> AllInception()
       {
           try
           {
               return objdb.AllInception();
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


       public long AddEditInception(InceptionModel objmodel)
       {
           try
           {
               return objdb.AddEditInception(objmodel);
           }
           catch (Exception)
           {
               return 0;
               throw;
           }
       }

       public InceptionModel GetInceptionById(int id)
       {
           try
           {
               return objdb.GetInceptionById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }


       public List<InceptionModel> GetInceptionListById(int id)
       {
           try
           {
               return objdb.GetInceptionListById(id);
           }
           catch (Exception)
           {
               return null;
               throw;
           }
       }
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class InceptionDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<InceptionModel> AllInception()
        {
            try
            {
                var ss = objdb.Inceptions.ToList();
                return ss.Select(x => new InceptionModel
                {
                    InceptionId = x.InceptionId,
                    MonthFrom = x.MonthFrom,
                    Code = x.TblUser.AgentCode,
                    RoleName = x.TblUser.Role.RoleName,
                   
                    ManagerName = objdb.TblUsers.Where(y=>y.UserId==x.TblUser.ManagerID).Select(y=>y.Name).SingleOrDefault(),
                    YearFrom = x.YearFrom,
      
                    AgentId = x.AgentId,
                    AgentName = x.TblUser.Name,
                    MTD_WAPI = x.MTD_WAPI,

                    YTD_WAPI = x.YTD_WAPI,

                }).OrderByDescending(x => x.InceptionId).ToList();
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

                if (objmodel.InceptionId == 0)
                {
                    Inception objInception = new Inception
                    {
                        InceptionId = objmodel.InceptionId,
                        MonthFrom = objmodel.MonthFrom,
                 
                        YearFrom = objmodel.YearFrom,
                       
                        AgentId = objmodel.AgentId,
                        MTD_WAPI = objmodel.MTD_WAPI,

                        YTD_WAPI = objmodel.YTD_WAPI,

                    };
                    objdb.Inceptions.Add(objInception);
                    objdb.SaveChanges();
                    return objInception.InceptionId;
                }
                else
                {
                    var objInception = objdb.Inceptions.Find(objmodel.InceptionId);
                    
                    objInception.InceptionId = objmodel.InceptionId;
                    //objInception.MonthFrom = objmodel.MonthFrom;
                    //objInception.MonthTo = objmodel.MonthTo;
                    //objInception.YearFrom = objmodel.YearFrom;
                    //objInception.YearTo = objmodel.YearTo;
                    objInception.AgentId = objmodel.AgentId;
                    objInception.MTD_WAPI = objmodel.MTD_WAPI;
                    objInception.YTD_WAPI = objmodel.YTD_WAPI;


                    objdb.SaveChanges();
                    return objmodel.InceptionId;
                }
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
                return objdb.Inceptions.Where(x => x.InceptionId == id).Select(x => new InceptionModel
                {
                    InceptionId = x.InceptionId,
                    MonthFrom = x.MonthFrom,
            
                    YearFrom = x.YearFrom,
              
                    AgentId = x.AgentId,
                    MTD_WAPI = x.MTD_WAPI,

                    YTD_WAPI = x.YTD_WAPI,

                }).SingleOrDefault();
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
                return objdb.Inceptions.Where(x => x.InceptionId == id).Select(x => new InceptionModel
                {
                    InceptionId = x.InceptionId,
                    MonthFrom = x.MonthFrom,
               
                    YearFrom = x.YearFrom,
         
                    AgentId = x.AgentId,
                    MTD_WAPI = x.MTD_WAPI,

                    YTD_WAPI = x.YTD_WAPI,

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


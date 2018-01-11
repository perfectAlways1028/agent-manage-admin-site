using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
    public class ManagerdistributeAmountDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<ManagerdistributeAmountModel> ManagerdistributeAmountList()
        {
            try
            {
                List<ManagerdistributeAmountModel> obj = new List<ManagerdistributeAmountModel>();
                obj = objdb.ManagerdistributeAmounts.Select(x => new ManagerdistributeAmountModel
                {
                    ID = x.ID,
                    AgentId = x.AgentId,
                    AgentName = x.TblUser.Name,
                    ManagerId = x.ManagerId,
                    Manager = x.TblUser1.Name,
                    Amount = x.Amount,
                    Date = x.Date,
                    IsActive = x.IsActive
                }).OrderByDescending(x => x.ID).ToList();
                return obj;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public List<ManagerdistributeAmountModel> GetManagerdistributeAmountListbyid(int id)
        {
            try
            {
                List<ManagerdistributeAmountModel> obj = new List<ManagerdistributeAmountModel>();
                var sobj = objdb.TblUsers.Where(x => x.ManagerID == id).ToList();
                obj = sobj.Select(x => new ManagerdistributeAmountModel
                {
                    ID = 0,
                    AgentId = x.UserId,
                    AgentName = x.Name,
                    ManagerId = x.ManagerID,
                    
                    Manager = objdb.TblUsers.Where(y=>y.UserId==x.ManagerID).Select(y=>y.Name).SingleOrDefault(),
                    Code = x.AgentCode,
                    Amount = 00,
                    IsActive = false
                }).OrderByDescending(x => x.ID).ToList();
                return obj;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public ManagerdistributeAmountModel GetAgentbyid(int id)
        {
            try
            {

                return objdb.TblUsers.Where(x => x.UserId == id).Select(x => new ManagerdistributeAmountModel
                {
                    
                    AgentId = x.UserId,
                    AgentName = x.Name,
                    ManagerId = x.ManagerID,
                    Manager = x.Name,
                    Amount = 00,
                    IsActive = false
                }).SingleOrDefault();

            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<ManagerdistributeAmountModel> GetManagerdistributeAmountAdmin()
        {
            try
            {
                List<ManagerdistributeAmountModel> obj = new List<ManagerdistributeAmountModel>();
                obj = objdb.ManagerdistributeAmounts.Select(x => new ManagerdistributeAmountModel
                {
                    ID = x.ID,
                    AgentId = x.AgentId,
                    AgentName = x.TblUser.Name,
                    ManagerId = x.ManagerId,
                    Manager = x.TblUser1.Name,
                    Amount = x.Amount,
                    Date = x.Date,
                    IsActive = x.IsActive
                }).OrderByDescending(x => x.ID).ToList();
                return obj;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public List<ManagerdistributeAmountModel> GetManagerdistributeAmountadminBydef()
        {
            try
            {
                List<ManagerdistributeAmountModel> obj = new List<ManagerdistributeAmountModel>();
                var sobj = objdb.TblUsers.Where(x => x.RoleId == 3 || x.RoleId == 4).ToList();
                obj = sobj.Select(x => new ManagerdistributeAmountModel
                {
                    ID = 0,
                    AgentId = x.UserId,
                    AgentName = x.Name,
                    ManagerId = x.ManagerID,
                    Code=x.AgentCode,
                    Manager = objdb.TblUsers.Where(y=>y.UserId==x.ManagerID).Select(y=>y.Name).SingleOrDefault(),
                    Amount = 00,
                    IsActive = false
                }).OrderByDescending(x => x.ID).ToList();
                return obj;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public List<ManagerdistributeAmountModel> GetManagerdistributeAmountListbyid2(int id)
        {
            try
            {
                List<ManagerdistributeAmountModel> obj = new List<ManagerdistributeAmountModel>();
                var sobj = objdb.ManagerdistributeAmounts.Where(x => x.ManagerId == id).ToList();
                obj = sobj.Select(x => new ManagerdistributeAmountModel
                {
                    ID = x.ID,
                    AgentId = x.AgentId,
                    AgentName = x.TblUser.Name,
                    ManagerId = x.ManagerId,
                    Manager = x.TblUser1.Name,
                    Code=x.TblUser.AgentCode,
                    Amount = x.Amount,
                    Date = x.Date,
                    IsActive = x.IsActive
                }).OrderByDescending(x => x.ID).ToList();
                return obj;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public ManagerdistributeAmountModel GetManagerdistributeAmountById(long id)
        {
            try
            {
                return objdb.ManagerdistributeAmounts.Where(x => x.ID == id).Select(x => new ManagerdistributeAmountModel
                {
                    ID = x.ID,
                    AgentId = x.AgentId,
                    AgentName = x.TblUser.Name,
                    ManagerId = x.ManagerId,
                    Manager = x.TblUser.Name,
                    Amount = x.Amount,
                    Date = x.Date,
                    IsActive = false
                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditManagerdistributeAmount(ManagerdistributeAmountModel model)
        {
            try
            {
                if (model.ID == 0)
                {
                    ManagerdistributeAmount obj = new ManagerdistributeAmount
                    {
                        AgentId = model.AgentId,
                        ManagerId = model.ManagerId,
                        Amount = model.Amount,
                        Date = model.Date,
                        IsActive = false
                    };
                    objdb.ManagerdistributeAmounts.Add(obj);
                    objdb.SaveChanges();
                    return obj.ID;
                }
                else
                {
                    ManagerdistributeAmount obj = objdb.ManagerdistributeAmounts.Find(model.ID);
                    if (obj != null)
                    {
                        obj.AgentId = model.AgentId;
                        obj.ManagerId = model.ManagerId;
                        obj.Amount = model.Amount;
                        obj.Date = model.Date;
                        obj.IsActive = false;
                        objdb.SaveChanges();
                        return obj.ID;
                    }
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public int DeleteManagerdistributeAmount(long id)
        {
            int res = 0;
            try
            {
                var pri = (from ok in objdb.ManagerdistributeAmounts where id == ok.ID select ok).Single();
                objdb.ManagerdistributeAmounts.Attach(pri);
                objdb.ManagerdistributeAmounts.Remove(pri);
                objdb.SaveChanges();
                res = 1;
                return res;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public bool ChangeManagerdistributeAmountsStatus(long id)
        {
            try
            {
                var obj = objdb.ManagerdistributeAmounts.Find(id);
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

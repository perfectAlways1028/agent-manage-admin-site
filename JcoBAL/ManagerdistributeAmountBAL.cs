using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JcoDAL;
using CommonUtility;

namespace JcoBAL
{
    public class ManagerdistributeAmountBAL
    {
        ManagerdistributeAmountDAL objdb = new ManagerdistributeAmountDAL();
        public List<ManagerdistributeAmountModel> ManagerdistributeAmountList()
        {
            try
            {
                return objdb.ManagerdistributeAmountList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditManagerdistributeAmount(ManagerdistributeAmountModel objmodel)
        {
            try
            {
                return objdb.AddEditManagerdistributeAmount(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public ManagerdistributeAmountModel GetManagerdistributeAmountById(int id)
        {
            try
            {
                return objdb.GetManagerdistributeAmountById(id);
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
                return objdb.GetManagerdistributeAmountListbyid(id);
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
                return objdb.GetManagerdistributeAmountadminBydef();
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
                return objdb.GetManagerdistributeAmountAdmin();
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
                return objdb.GetManagerdistributeAmountListbyid2(id);
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
                return objdb.GetAgentbyid(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public bool ChangeManagerdistributeAmountsStatus(int id)
        {
            try
            {
                return objdb.ChangeManagerdistributeAmountsStatus(id);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}

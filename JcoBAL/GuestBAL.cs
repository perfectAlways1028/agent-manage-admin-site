using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;
using JcoDAL;

namespace JcoBAL
{
   public class GuestBAL
    {
        GuestDAL objdb = new GuestDAL();
        public List<GuestModel> AllGuest()
        {
            try
            {
                return objdb.AllGuest();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public long AddEditGuest(GuestModel objmodel)
        {
            try
            {
                return objdb.AddEditGuest(objmodel);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public GuestModel GetGuestById(int id)
        {
            try
            {
                return objdb.GetGuestById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<GuestModel> GetGuestListById(int id)
        {
            try
            {
                return objdb.GetGuestListById(id);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}

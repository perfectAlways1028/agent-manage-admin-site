using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility;

namespace JcoDAL
{
   public class DutyDAL
    {
        JCOEntities objdb = new JCOEntities();
        public List<DutyModel> AllDuty()
        {
            try
            {
                return objdb.Duties.Select(x => new DutyModel
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    Description = x.Description,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    RoadshowCode = x.RoadshowCode,
                    Cost = x.Cost,
                }).OrderByDescending(x => x.DutyId).ToList();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public long AddEditDuty(DutyModel objmodel)
        {
            try
            {

                if (objmodel.DutyId == 0)
                {
                    Duty objDuty = new Duty
                    {
                        Location = objmodel.Location,
                        Description = objmodel.Description,
                        DateFrom = objmodel.DateFrom,
                        DateTo = objmodel.DateTo,
                        RoadshowCode = objmodel.RoadshowCode,
                        Cost = objmodel.Cost,
                   
                    };
                    objdb.Duties.Add(objDuty);
                    objdb.SaveChanges();
                    return objDuty.DutyId;
                }
                else
                {
                    var objDuty = objdb.Duties.Find(objmodel.DutyId);
                    objDuty.Location = objmodel.Location;
                    objDuty.DutyId = objmodel.DutyId;
                    objDuty.Description = objmodel.Description;
                    objDuty.DateFrom = objmodel.DateFrom;
                    objDuty.DateTo = objmodel.DateTo;
                    objDuty.RoadshowCode = objmodel.RoadshowCode;
                    objDuty.Cost = objmodel.Cost;
                   
                    objdb.SaveChanges();
                    return objmodel.DutyId;
                }
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public DutyModel GetDutyById(int id)
        {
            try
            {
                return objdb.Duties.Where(x => x.DutyId == id).Select(x => new DutyModel
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    Description = x.Description,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    RoadshowCode = x.RoadshowCode,
                    Cost = x.Cost,
                    //AgentId = x.AgentId,
                }).SingleOrDefault();
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<DutyModel> GetDutyListById(int id)
        {
            try
            {
                return objdb.Duties.Where(x => x.DutyId == id).Select(x => new DutyModel
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    Description = x.Description,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    RoadshowCode = x.RoadshowCode,
                    Cost = x.Cost,
                    //AgentId = x.AgentId,
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


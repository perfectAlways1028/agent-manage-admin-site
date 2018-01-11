using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonUtility;
using JcoDAL;
using JcoBAL;
using System.Net.Mail;
using System.Net;
using System.Xml.Linq;
using Jco.Filters;
using Jco.Models;
using System.Web.UI.WebControls;
using LinqToExcel;
using System.Data;
//using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Web.UI;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Text;


namespace Jco.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        JCOEntities objdb = new JCOEntities();
        TimeZoneInfo IND_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        string adminEmail = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];
        string MailEmail = System.Configuration.ConfigurationSettings.AppSettings["MailUid"];
        string MailPassword = System.Configuration.ConfigurationSettings.AppSettings["MailPwd"];
        string host = System.Configuration.ConfigurationSettings.AppSettings["smtpAddress"];
        string MainURL = System.Configuration.ConfigurationSettings.AppSettings["MainURL"];

        #region login
        public ActionResult Index()
        {
            UserModel model = new UserModel();
            model.CompanyProfileList = new CompanyProfileBAL { }.AllCompanyProfile();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(UserModel model)
        {
            try
            {
                var user = new UserBAL { }.Login(model.EmailId, DataEncryption.Encrypt(model.Password.Trim(), "passKey"));
                if (user != null)
                {

                    Session["UserId"] = user.UserId;
                    Session["RoleId"] = user.RoleId;
                    Session["EmailId"] = user.EmailId;
                    Session["Name"] = user.Name;
                 
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    Session["Invalid"] = "Invalid User";
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        public ActionResult Logout()
        {
            try
            {
                Response.Buffer = true;
                Response.Expires = -1500;
                Response.CacheControl = "no-cache";
                Session.Abandon();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult ForgotPassword(UserModel model)
        {
            try
            {
                var user = objdb.TblUsers.Where(x=>x.EmailId==model.EmailId).SingleOrDefault();

                if (user != null)
                {
                    string pwd = DataEncryption.Decrypt(user.Password.Trim(), "passKey");
                   
                    MailMessage msg = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    Attachment MyAttachment = null;
                    string emailId = model.EmailId;
                    //string uPwd = model.Password.Trim();
                    msg.From = new MailAddress("noreply@AWARDBRUSH.COM");
                    //Receiver email address
                    msg.To.Add(emailId);
                    msg.Subject = "JCO Forgot Password Request";
                    //string cs = "http://creditlink.kazmatechnology.in/Home/LOIAccepted/" + cand.CandidateId;
                    ////string cs = "http://localhost:2997/Home/LOIAccepted/" +CandidateId;
                    //string link = "<a  href='" + cs + "'>Accept Or Ammendment</a>";
                    string mailbod = "Your JCO Login Password is: " + pwd;

                    //string mailb = templ.VendorRequestTemplate(cand.FirstName, mailbod, "LOI Credit Link");
                    msg.Body = mailbod;
                    msg.IsBodyHtml = true;
                    smtp.Credentials = new NetworkCredential(MailEmail, MailPassword);
                    smtp.Port = 25;
                    smtp.Host = host;
                    smtp.EnableSsl = false;
                    smtp.Send(msg);

                    Session["Success"] = "Please check your registered mail Id";
                }
                else
                {
                   
                    Session["Invalid"] = "Invalid User";
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }
        #endregion login

        public ActionResult Dashboard()
        {
            return View();
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        //public JsonResult GetDiarySummary(double start, double end)
        //{
        //    var ApptListForDate = DiaryEvent.LoadAppointmentSummaryInDateRange(start, end);
        //    var eventList = from e in ApptListForDate
        //                    select new
        //                    {
        //                        id = e.ID,
        //                        title = e.Title,
        //                        start = e.StartDateString,
        //                        end = e.EndDateString,
        //                        someKey = e.SomeImportantKeyID,
        //                        allDay = false
        //                    };
        //    var rows = eventList.ToArray();
        //    return Json(rows, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetDiaryEvents(double start, double end)
        //{
        //    var ApptListForDate = DiaryEvent.LoadAllAppointmentsInDateRange(start, end);
        //    var eventList = from e in ApptListForDate
        //                    select new
        //                    {
        //                        id = e.ID,
        //                        title = e.Title,
        //                        start = e.StartDateString,
        //                        end = e.EndDateString,
        //                        color = e.StatusColor,
        //                        className = e.ClassName,
        //                        someKey = e.SomeImportantKeyID,
        //                        allDay = false
        //                    };
        //    var rows = eventList.ToArray();
        //    return Json(rows, JsonRequestBehavior.AllowGet);
        //}
        #region  User
        public ActionResult User()
        {
            UserModel model = new UserModel();

            model.UserList = new UserBAL { }.AllUser();
            CustomMethods.ValidateRoles("User");
            return View(model);
        }
        public ActionResult AddEditUser(int id = 0)
        {
            UserModel model = new UserModel();
            if (id != 0)
            {
                UserModel obj = new UserBAL { }.GetUserById(id);
                if (obj != null)
                {
                    model.UserId = obj.UserId;
                    model.EmailId = obj.EmailId;
                    model.Name = obj.Name;
                    model.Password = DataEncryption.Decrypt(obj.Password.Trim(), "passKey");
                    model.Mobile = obj.Mobile;
                    model.Photo = obj.Photo;
                    model.AgentCode = obj.AgentCode;
                    model.JoinDate = obj.JoinDate;
                    model.Designation = obj.Designation;
                    model.ManagerID = obj.ManagerID;
                    model.Gender = obj.Gender;
                    model.DOB = obj.DOB;
                    model.CreatedBy = obj.CreatedBy;
                    model.IsActive = obj.IsActive;
                    model.RoleId = obj.RoleId;
                }
              
            }
            List<SelectListItem> NewList = new List<SelectListItem>();
            NewList.Add(new SelectListItem { Text = "Male", Value = "Male" });
            NewList.Add(new SelectListItem { Text = "Female", Value = "Female" });
            model.ListGender = NewList;

            var roleslist = new RolesBAL { }.GetAllRoles();
            if (roleslist != null)
            {
                model.GetType().GetProperty("Roles").SetValue(model, roleslist.Select(x => new SelectListItem { Value = x.RoleId.ToString(), Text = x.RoleName }));
            }

            var ManagerList = objdb.TblUsers.Where(x=>x.RoleId==4).ToList();
            if (ManagerList != null)
            {
                model.GetType().GetProperty("ManagerList").SetValue(model, ManagerList.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }));
            }

            CustomMethods.ValidateRoles("User");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditUser(UserModel model, HttpPostedFileBase file)
        {
            UserBAL catBLL = new UserBAL();
            string strFileName = "";
            string path = "";
            long userid = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                if (file != null)
                {
                    strFileName = "UserImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/Users/" + strFileName);
                    file.SaveAs(path);
                }
                res = new UserBAL { }.AddEditUser(new UserModel
                {
                    UserId = model.UserId,
                    EmailId = model.EmailId,
                    Password = DataEncryption.Encrypt(model.Password.Trim(), "passKey"),
                    Mobile = model.Mobile,
                    Name = model.Name,
                    Photo = strFileName,
                    AgentCode = model.AgentCode,
                    JoinDate = model.JoinDate,
                    ManagerID = model.ManagerID,
                    Designation = model.Designation,
                    Gender = model.Gender,
                    DOB = model.DOB,
                    CreatedBy = userid,
                    UpdatedBy = userid,
                    IsActive = model.IsActive,
                    RoleId = model.RoleId,
                  
                });
                if (res != 0)
                {
                    if (model.UserId == 0)
                    {
                        Session["Success"] = "User added successfully!";
                    }
                    else {
                        Session["Success"] = "User Updated successfully!";
                    }
                    return RedirectToAction("User");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }
        public JsonResult CheckDuplicate(string EmailId,long UserId)
        {
            bool result = false;

            try
            {
                if (UserId > 0)
                {
                    var obj = objdb.TblUsers.Where(x => x.EmailId == EmailId && x.UserId != UserId).FirstOrDefault();
                    if (obj != null)
                    {
                        result = true;
                    }

                }
                else
                {
                    result = new UserBAL { }.CheckDuplicateEmail(EmailId.Trim());
                }

               

                if (result == true)
                {
                    return Json(result);
                }
                else
                {
                    return Json(result);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public JsonResult CheckDuplicateCode(string code, long UserId)
        {
            bool result = false;

            try
            {
                if (UserId > 0)
                {
                    var obj = objdb.TblUsers.Where(x => x.AgentCode == code && x.UserId!=UserId).FirstOrDefault();
                    if (obj != null)
                    {
                        result = true;
                    }

                }
                else 
                {
                    result = new UserBAL { }.CheckDuplicateAgentCode(code.Trim());
                }
               

                if (result == true)
                {
                    return Json(result);
                }
                else
                {
                    return Json(result);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public JsonResult CheckDuplicateMob(string Mobile, long UserId)
        {
            bool result = false;

            try
            {
                if (UserId > 0)
                {
                    var obj = objdb.TblUsers.Where(x => x.Mobile == Mobile && x.UserId != UserId).FirstOrDefault();
                    if (obj != null)
                    {
                        result = true;
                    }

                }
                else
                {
                    result = new UserBAL { }.CheckDuplicateMob(Mobile.Trim());
                }
            

                if (result == true)
                {
                    return Json(result);
                }
                else
                {
                    return Json(result);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion User

        #region  Poll
        public ActionResult Poll()
        {
            PollModel model = new PollModel();
            model.PollList = new PollBAL { }.AllPoll();
            CustomMethods.ValidateRoles("Poll");
            return View(model);
        }
        public ActionResult AddEditPoll(int id = 0)
        {
            PollModel model = new PollModel();
            if (id != 0)
            {
                PollModel obj = new PollBAL { }.GetPollById(id);
                if (obj != null)
                {
                    model.PollId = obj.PollId;
                    model.Title = obj.Title;
                    model.Description = obj.Description;
                    model.VoteLastDate = obj.VoteLastDate;
                    model.VoteLastDateBind = obj.VoteLastDate.ToString("yyyy-MM-dd");
                    model.CreatedBy = obj.CreatedBy;
                    model.Image = obj.Image;
                    model.IsActive = obj.IsActive;
                }
                model.PollOptionList = new PollOptionBAL { }.GetPollOptionListById(id);

                var list1 = objdb.TblUsers.Where(x => x.RoleId == 3 || x.RoleId == 4).Select(x => new
                {
                    AgentId = x.UserId,
                    Name = x.Name,
                    AgentCode = x.AgentCode,
                    ManagerID = x.ManagerID,
                    EmailId=x.EmailId,
                    Mobile=x.Mobile,
                    Gender=x.Gender,
                    RoleName = x.Role.RoleName,
                    Id = 0,
                }).ToList();

                var list2 = objdb.AssignPollingAgents.Where(x => x.PollId == id).Select(x => new
                {
                    AgentId = x.AgentId,
                    AgentCode = x.TblUser.AgentCode,
                    Name = x.TblUser.Name,
                    EmailId = x.TblUser.EmailId,
                    ManagerID = x.TblUser.ManagerID,
                    Mobile = x.TblUser.Mobile,
                    Gender = x.TblUser.Gender,
                    RoleName = x.TblUser.Role.RoleName,
                    AssignPollingAgentId = x.AssignPollingAgentId,
                }).ToList();

                var joinedList = (from s in list1
                                  join cs in list2
                                  on s.AgentId equals cs.AgentId into studentInfo
                                  from students in studentInfo.DefaultIfEmpty()
                                  select new
                                  {
                                      s.AgentId,
                                      s.Name,
                                      s.EmailId,
                                      s.Mobile,
                                      s.Gender,
                                      s.RoleName,
                                      s.AgentCode,
                                      s.ManagerID,
                                      AssignPollingAgentId = students == null ? 0 : students.AssignPollingAgentId 
                                  }).Distinct().ToList();
                //model.AgentList = joinedList.Where(x => x.AssignPollingAgentId == 0).Select(x => new UserModel
                model.AgentList = joinedList.Select(x => new UserModel
                {
                    UserId = x.AgentId,
                    Name = x.Name,

                    EmailId = x.EmailId,
                    Mobile = x.Mobile,
                    Gender = x.Gender,
                    AgentCode = x.AgentCode,
                    RoleName = x.RoleName,
                    ManagerName=objdb.TblUsers.Where(y=>y.UserId==x.ManagerID).Select(y=>y.Name).SingleOrDefault(),
                    AssignCommonId=x.AssignPollingAgentId
                }).OrderBy(x => x.Name).ToList();
            }
            else 
            {
                model.AgentList = new UserBAL { }.AllUser().Where(x => x.RoleId == 3 || x.RoleId == 4).ToList();
            }


            CustomMethods.ValidateRoles("Poll");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditPoll(PollModel model, FormCollection form, HttpPostedFileBase file)
        {
            PollBAL catBLL = new PollBAL();
            string strFileName = "";
            string path = "";
            long Pollid = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                if (file != null)
                {
                    strFileName = "UserImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/Polling/" + strFileName);
                    file.SaveAs(path);
                }
                res = new PollBAL { }.AddEditPoll(new PollModel
                {
                    PollId = model.PollId,
                    Title = model.Title,
                    Description = model.Description,
                    VoteLastDate = Convert.ToDateTime(model.VoteLastDateBind),
                    Image=strFileName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Pollid,
                    IsActive = model.IsActive,

                });
                if (res != 0)
                {
                    //clear dbo.AssignPollingAgent
                    objdb.AssignPollingAgents.Where(p => p.PollId == res)
                       .ToList().ForEach(p => objdb.AssignPollingAgents.Remove(p));
                    objdb.SaveChanges();

                    string ChkId = System.Web.HttpContext.Current.Request.Form["sid"];
                    if (ChkId != null)
                    {
                        string[] ChkIds = ChkId.Split(',').Select(sValue => sValue.Trim()).ToArray();
                        foreach (var item in ChkIds)
                        {
                            new AssignPollingAgentBAL { }.AddEditAssignPollingAgent(new AssignPollingAgentModel
                            {
                                PollId = res,
                                AgentId = Convert.ToInt32(item),

                            });
                            //SendNotification(Convert.ToInt32(item),"New Poll",model.Title);
                        }
                    }



                    if (model.PollId == 0)
                    {
                        Session["Success"] = "Polling added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Polling Updated successfully!";
                    }
                    //clear dbo.voting options

                  
                    objdb.PollOptions.Where(p => p.PollId == res)
                       .ToList().ForEach(p => objdb.PollOptions.Remove(p));
                    objdb.SaveChanges();

                    string[] Options = Request.Form.GetValues("Options");
                    var Answer = form["Answer"];
                    
                    if (Options != null)
                    {
                        int i = 1;
                        foreach (var item in Options)
                        {
                            if (i == Convert.ToInt32(Answer))
                            {
                                new PollOptionBAL { }.AddEditPollOption(new PollOptionModel
                                {
                                    Options = item,
                                    PollId = res,
                                    IsTrue = true,

                                });
                            }
                            else
                            {
                                new PollOptionBAL { }.AddEditPollOption(new PollOptionModel
                                {
                                    Options = item,
                                    PollId = res,

                                });
                            }

                            i++;
                        }
                    }

                    return RedirectToAction("Poll");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }
        public ActionResult ChangePollStatus(int id)
        {
            bool Result = false;
            bool ChangeStatus = new PollOptionBAL { }.ChangePollStatus(id);
            if (ChangeStatus)
            {
                Result = true;
            }
            TempData["msg"] = "<script>alert('Status Change Successfully');</script>";
            return RedirectToAction("Poll", "Home");
        }
        public void OptionDel(long Id)
        {
            var crt = objdb.PollOptions.Where(x => x.PollOptionId == Id).SingleOrDefault();
            objdb.PollOptions.Remove(crt);
            objdb.SaveChanges();

        }

        public ActionResult TotalPoll(long PollId)
        {
            PollResultModel model = new PollResultModel();
            var pl = objdb.Polls.Where(x => x.PollId == PollId).SingleOrDefault();
            model.PollTitle = pl.Title;
            //model.PollResultList = new PollResultBAL { }.AllPoll().Where(x => x.PollId == PollId).ToList();
            var list = objdb.AssignPollingAgents.Where(x => x.PollId == PollId).ToList();
            var list2 = list.Where(x => x.PollId == PollId).Select(x => new
            {
                AgentId = x.AgentId,
                Name = x.TblUser.Name,
                ManagerID = x.TblUser.ManagerID,
                EmailId = x.TblUser.EmailId,
                Mobile = x.TblUser.Mobile,
                code = x.TblUser.AgentCode,
                Gender = x.TblUser.Gender,
                RoleName = x.TblUser.Role.RoleName,
                ResultId = new PollResultBAL { }.AllPoll().Where(y => y.PollId == PollId && y.AgentId == x.AgentId).Select(y => y.ResultId).SingleOrDefault(),
                Option = new PollResultBAL{ }.AllPoll().Where(y=>y.PollId==PollId && y.AgentId==x.AgentId).Select(y=>y.Options).SingleOrDefault(),
                Answer = new PollResultBAL { }.AllPoll().Where(y => y.PollId == PollId && y.AgentId == x.AgentId).Select(y => y.Answer).SingleOrDefault(),
            }).ToList();

            model.PollResultList = list2.Select(x => new PollResultModel
            {
                AgentId = x.AgentId,
               
                AgentName = x.Name,
                EmailId = x.EmailId,
                Mobile = x.Mobile,
                Gender = x.Gender,
                RoleName = x.RoleName,
                Options = x.Option,
                Answer = x.Answer,
                Code=x.code,
                ResultId = x.ResultId,
                ManagerName = objdb.TblUsers.Where(y => y.UserId == x.ManagerID).Select(y => y.Name).SingleOrDefault(),
            }).ToList();
            CustomMethods.ValidateRoles("Poll");
            return View(model);
        }
        #endregion Poll

        #region  Event
        public ActionResult Event()
        {
            EventModel model = new EventModel();
            model.EventList = new EventBAL { }.AllEvent();
            CustomMethods.ValidateRoles("Event");
            return View(model);
        }
        public ActionResult EventAgent(long Id)
        {
            AgentEventParticipateModel model = new AgentEventParticipateModel();
            var evt = objdb.AgentEventParticipates.Where(x=>x.EventId==Id).ToList();
            model.AgentEventParticipateList = evt.Select(x => new AgentEventParticipateModel
            {
                EventId = x.Event.EventId,
                EventName = x.Event.EventName,
                AgentName = x.TblUser.Name,
                Date = x.Date,
            }).OrderByDescending(x => x.EventId).ToList();

            CustomMethods.ValidateRoles("Event");
            return View(model);
        }

        public ActionResult EventGuest(long Id)
        {
            GuestModel model = new GuestModel();
            model.GuestList = new GuestBAL { }.AllGuest().Where(x => x.EventId == Id).ToList();

            CustomMethods.ValidateRoles("Event");
            return View(model);
        }
        public ActionResult AddEditEvent(int id = 0)
        {
            EventModel model = new EventModel();
            if (id != 0)
            {
                EventModel obj = new EventBAL { }.GetEventById(id);
                if (obj != null)
                {
                    model.EventId = obj.EventId;
                    model.EventName = obj.EventName;
                    model.Description = obj.Description;
                    model.EventDate = obj.EventDate;
                    model.EventDateBind = obj.EventDate.ToString("yyyy-MM-dd");
                    model.StartTime = obj.StartTime;
                    model.EndTime = obj.EndTime;
                    model.AgentSeat = obj.AgentSeat;
                    model.IsGuestRegistration = obj.IsGuestRegistration;
                    model.GuestSeat = obj.GuestSeat;
                    model.RSVP_By = obj.RSVP_By;
                    model.RSVP_By_Bind = obj.RSVP_By.ToString("yyyy-MM-dd");
                    model.RSVP_Time = obj.RSVP_Time;
                    model.CreatedBy = obj.CreatedBy;
                  
                }

            }
            CustomMethods.ValidateRoles("Event");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditEvent(EventModel model, HttpPostedFileBase file)
        {
            EventBAL catBLL = new EventBAL();
            string strFileName = "";
            string path = "";
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                if (file != null)
                {
                    strFileName = "EventImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/Events/" + strFileName);
                    file.SaveAs(path);
                }
                res = new EventBAL { }.AddEditEvent(new EventModel
                {
                    EventId = model.EventId,
                    EventName = model.EventName,
                    Description = model.Description,

                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    AgentSeat = model.AgentSeat,

                    IsGuestRegistration = model.IsGuestRegistration,
                    GuestSeat = model.GuestSeat,
                    RSVP_By = Convert.ToDateTime(model.RSVP_By_Bind),
                    RSVP_Time = model.RSVP_Time,

                    CreatedBy = UserId,
                    EventDate = Convert.ToDateTime(model.EventDateBind),

                });
                if (res != 0)
                {
                    if (model.EventId == 0)
                    {
                        Session["Success"] = "Event added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Event Updated successfully!";
                    }
                    return RedirectToAction("Event");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }

        #endregion Event

        #region  Announcement
        public ActionResult Announcement()
        {
            AnnouncementModel model = new AnnouncementModel();
            model.AnnouncementList = new AnnouncementBAL { }.AllAnnouncement();
            CustomMethods.ValidateRoles("Announcement");
            return View(model);
        }
        public ActionResult AddEditAnnouncement(int id = 0)
        {
            AnnouncementModel model = new AnnouncementModel();
            if (id != 0)
            {
                AnnouncementModel obj = new AnnouncementBAL { }.GetAnnouncementById(id);
                if (obj != null)
                {
                    model.AnnouncementId = obj.AnnouncementId;
                    model.Title = obj.Title;
                    model.Description = obj.Description;
                    model.PublishDate = obj.PublishDate;
                    model.PublishDateBind = obj.PublishDate.ToString("yyyy-MM-dd");
                    model.Image = obj.Image;
                    model.ExpireDate = obj.ExpireDate;
                    model.ExpireDateBind = obj.ExpireDate.ToString("yyyy-MM-dd");
                    model.CreatedBy = obj.CreatedBy;

                }
                var list1 = objdb.TblUsers.Where(x => x.RoleId == 3 || x.RoleId == 4).Select(x => new
                {
                    AgentId = x.UserId,
                    Name = x.Name,
                    ManagerId=x.ManagerID,
                    AgentCode=x.AgentCode,
                    EmailId = x.EmailId,
                    Mobile = x.Mobile,
                    Gender = x.Gender,
                    RoleName = x.Role.RoleName,
                    Id = 0,
                }).ToList();

                var list2 = objdb.AssignAnnouncements.Where(x => x.AnnouncementId == id).Select(x => new
                {
                    AgentId = x.UserId,
                    Name = x.TblUser.Name,
                    ManagerId = x.TblUser.ManagerID,
                    AgentCode = x.TblUser.AgentCode,
                    EmailId = x.TblUser.EmailId,
                    Mobile = x.TblUser.Mobile,
                    Gender = x.TblUser.Gender,
                    RoleName = x.TblUser.Role.RoleName,
                    AssignAnnouncementId = x.AssignAnnouncementId,
                }).ToList();

                var joinedList = (from s in list1
                                  join cs in list2
                                  on s.AgentId equals cs.AgentId into studentInfo
                                  from students in studentInfo.DefaultIfEmpty()
                                  select new
                                  {
                                      s.AgentId,
                                      s.Name,
                                      s.EmailId,
                                      s.Mobile,
                                      s.Gender,
                                      s.RoleName,
                                      s.ManagerId,
                                      s.AgentCode,
                                      AssignAnnouncementId = students == null ? 0 : students.AssignAnnouncementId
                                  }).Distinct().ToList();
                //model.AgentList = joinedList.Where(x => x.AssignPollingAgentId == 0).Select(x => new UserModel
                model.AgentList = joinedList.Select(x => new UserModel
                {
                    UserId = x.AgentId,
                    Name = x.Name,
                    AgentCode = x.AgentCode,

                    EmailId = x.EmailId,
                    Mobile = x.Mobile,
                    Gender = x.Gender,
                    RoleName = x.RoleName,
                    ManagerName = objdb.TblUsers.Where(y => y.UserId == x.ManagerId).Select(y => y.Name).SingleOrDefault(),
                    AssignCommonId = x.AssignAnnouncementId
                }).OrderBy(x => x.Name).ToList();

            }
            else {
                model.AgentList = new UserBAL { }.AllUser().Where(x => x.RoleId == 3 || x.RoleId == 4).ToList();
            }
          
            CustomMethods.ValidateRoles("Announcement");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditAnnouncement(AnnouncementModel model, HttpPostedFileBase file)
        {
            AnnouncementBAL catBLL = new AnnouncementBAL();
            string strFileName = "";
            string path = "";
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
               
                long res = 0;
                if (file != null)
                {
                    strFileName = "AnnouncementImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/Announcements/" + strFileName);
                    file.SaveAs(path);
                }
                res = new AnnouncementBAL { }.AddEditAnnouncement(new AnnouncementModel
                {
                    AnnouncementId = model.AnnouncementId,
                    Title = model.Title,
                    Description = model.Description,
                    PublishDate = Convert.ToDateTime(model.PublishDateBind),
                    ExpireDate = Convert.ToDateTime(model.ExpireDateBind),
                    Image = strFileName,

                    CreatedBy = UserId,

                });
                if (res != 0)
                {
                    //clear dbo.AssignAnnouncements
                    objdb.AssignAnnouncements.Where(p => p.AnnouncementId == res)
                       .ToList().ForEach(p => objdb.AssignAnnouncements.Remove(p));
                    objdb.SaveChanges();

                    string ChkId = System.Web.HttpContext.Current.Request.Form["sid"];
                    string[] ChkIds = ChkId.Split(',').Select(sValue => sValue.Trim()).ToArray();
                    foreach (var item in ChkIds)
                    {
                        new AssignAnnouncementBAL { }.AddEditAssignAnnouncement(new AssignAnnouncementModel
                        {
                            AnnouncementId = res,
                            UserId = Convert.ToInt32(item),

                        });

                        //SendNotification(Convert.ToInt32(item),"New Announcement",model.Title);
                    }



                    if (model.AnnouncementId == 0)
                    {
                        Session["Success"] = "Announcement added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Announcement Updated successfully!";
                    }
                    return RedirectToAction("Announcement");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }

        #endregion Announcement

        #region  Duty
        public ActionResult Duty()
        {
            DutyModel model = new DutyModel();
            model.DutyList = new DutyBAL { }.AllDuty();
            CustomMethods.ValidateRoles("Duty");
            return View(model);
        }
        public ActionResult AddEditDuty(int id = 0)
        {
            DutyModel model = new DutyModel();
            if (id != 0)
            {
                DutyModel obj = new DutyBAL { }.GetDutyById(id);
                if (obj != null)
                {
                    model.DutyId = obj.DutyId;
                    model.Location = obj.Location;
                    model.Description = obj.Description;
                    model.DateFrom = obj.DateFrom;
                    model.DutyDateBind = obj.DateFrom.ToString("yyyy-MM-dd");
                    model.DateTo = obj.DateTo;
                    model.DateFromStr = obj.DateFrom.ToString("yyyy-MM-dd");
                    model.DateToStr = obj.DateTo.ToString("yyyy-MM-dd");
                    model.RoadshowCode = obj.RoadshowCode;
                    model.Cost = obj.Cost;
                    model.AgentId = obj.AgentId;

                }

            }
            CustomMethods.ValidateRoles("Duty");
            var agents = new UserBAL { }.AllUser().Where(x=>x.RoleId==3).ToList();
            if (agents != null)
            {
                model.GetType().GetProperty("ListAgent").SetValue(model, agents.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }));
            }
            return View(model);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditDuty(DutyModel model)
        {
            DutyBAL catBLL = new DutyBAL();
            string strFileName = "";
            string path = "";
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            model.DateFrom = Convert.ToDateTime(model.DateFromStr);
            model.DateTo = Convert.ToDateTime(model.DateToStr);
            Random rnd = new Random();
            try
            {
                long res = 0;
             
                res = new DutyBAL { }.AddEditDuty(new DutyModel
                {
                    DutyId = model.DutyId,
                    Location = model.Location,
                    Description = model.Description,

                    DateTo = model.DateTo,
                    RoadshowCode = model.RoadshowCode,
                    Cost = model.Cost,
                    AgentId = model.AgentId,
                    DateFrom = model.DateFrom,

                });
                if (res != 0)
                {
                    if (model.DutyId == 0)
                    {
                        Session["Success"] = "Duty added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Duty Updated successfully!";
                    }
                    return RedirectToAction("Duty");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }


        public ActionResult AssignDuty(int id)
        {
            DutyModel model = new DutyModel();
            if (id != 0)
            {
                DutyModel obj = new DutyBAL { }.GetDutyById(id);
                if (obj != null)
                {
                    model.DutyId = obj.DutyId;
                    model.Location = obj.Location;
                    model.Description = obj.Description;
                    model.DateFrom = obj.DateFrom;
                    model.DutyDateBind = obj.DateFrom.ToString("yyyy-MM-dd");
                    model.DateTo = obj.DateTo;
                    model.RoadshowCode = obj.RoadshowCode;
                    model.Cost = obj.Cost;
                    model.AgentId = obj.AgentId;

                }
                //model.AssignDutyList = new AssignDutyBAL { }.GetAssignDutyListById(id);
                var list1 = objdb.TblUsers.Where(x => x.RoleId == 3 || x.RoleId == 4).Select(x => new
                {
                    AgentId = x.UserId,
                    Name = x.Name,
                    ManagerId = x.ManagerID,
                    AgentCode = x.AgentCode,
                    EmailId = x.EmailId,
                    Mobile = x.Mobile,
                    Gender = x.Gender,
                    RoleName = x.Role.RoleName,
                    Id = 0,
                }).ToList();

                var list2 = objdb.AssignDuties.Where(x => x.DutyId == id).Select(x => new
                {
                    AgentId = x.AgentId,
                    Name = x.TblUser.Name,
                    ManagerId = x.TblUser.ManagerID,
                    AgentCode = x.TblUser.AgentCode,
                    EmailId = x.TblUser.EmailId,
                    Mobile = x.TblUser.Mobile,
                    Gender = x.TblUser.Gender,
                    RoleName = x.TblUser.Role.RoleName,
                    AssignDutyId = x.AssignDutyId,
                }).ToList();

                var joinedList = (from s in list1
                                  join cs in list2
                                  on s.AgentId equals cs.AgentId into studentInfo
                                  from students in studentInfo.DefaultIfEmpty()
                                  select new
                                  {
                                      s.AgentId,
                                      s.Name,
                                      s.EmailId,
                                      s.Mobile,
                                      s.Gender,
                                      s.RoleName,
                                      s.ManagerId,
                                      s.AgentCode,
                                      AssignDutyId = students == null ? 0 : students.AssignDutyId
                                  }).Distinct().ToList();
                //model.AgentList = joinedList.Where(x => x.AssignPollingAgentId == 0).Select(x => new UserModel
                model.AgentList = joinedList.Select(x => new UserModel
                {
                    UserId = x.AgentId,
                    Name = x.Name,
                    AgentCode = x.AgentCode,

                    EmailId = x.EmailId,
                    Mobile = x.Mobile,
                    Gender = x.Gender,
                    RoleName = x.RoleName,
                    ManagerName = objdb.TblUsers.Where(y => y.UserId == x.ManagerId).Select(y => y.Name).SingleOrDefault(),
                    AssignCommonId = x.AssignDutyId
                }).OrderBy(x => x.Name).ToList();


            }
            CustomMethods.ValidateRoles("Duty");


            //var list1 = objdb.TblUsers.Where(x => x.RoleId == 3).Select(x => new
            //{
            //    AgentId = x.UserId,
            //    Name = x.Name,
            //    DutyId = 0,
            //}).ToList();

            //var list2 = objdb.AssignDuties.Where(x => x.DutyId == id).Select(x => new
            //{
            //    AgentId = x.AgentId,
            //    Name = x.TblUser.Name,
            //    DutyId = id,
            //}).ToList();
            //var joinedList = (from s in list1
            //                  join cs in list2
            //                  on s.AgentId equals cs.AgentId into studentInfo
            //                  from students in studentInfo.DefaultIfEmpty()
            //                  select new
            //                  {
            //                      s.AgentId,
            //                      s.Name,
            //                      DutyId = students == null ? 0 : students.DutyId
            //                  }).Distinct().ToList();
            //var listtt = joinedList.Where(x => x.DutyId == 0).Select(x => new UserModel
            //{
            //    UserId = x.AgentId,
            //    Name = x.Name,
            //}).OrderBy(x => x.Name).ToList();

            //if (listtt != null)
            //{
            //    model.GetType().GetProperty("ListAgent").SetValue(model, listtt.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }));
            //}



            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AssignDuty(DutyModel model)
        {

            long UserId = Convert.ToInt32(Session["UserId"].ToString());

            try
            {
                //clear dbo.AssignAnnouncements
                objdb.AssignDuties.Where(p => p.DutyId == model.DutyId)
                   .ToList().ForEach(p => objdb.AssignDuties.Remove(p));
                objdb.SaveChanges();

                string ChkId = System.Web.HttpContext.Current.Request.Form["sid"];
                string[] ChkIds = ChkId.Split(',').Select(sValue => sValue.Trim()).ToArray();
                foreach (var item in ChkIds)
                {
                    new AssignDutyBAL { }.AddEditAssignDuty(new AssignDutyModel
                    {
                        DutyId = model.DutyId,
                        AgentId = Convert.ToInt32(item),

                    });

                    //SendNotification(Convert.ToInt32(item),"New Announcement",model.Title);
                }
                //long res = 0;

                //res = new AssignDutyBAL { }.AddEditAssignDuty(new AssignDutyModel
                //{
                //    DutyId = model.DutyId,
                //    AgentId = Convert.ToInt32(model.AgentId),

                //});
                //if (res != 0)
                //{

                //    Session["Success"] = "Duty Assigned successfully!";


                //    return RedirectToAction("AssignDuty", new { id = model.DutyId });
                //}
                Session["Success"] = "Duty Assigned successfully!";
                return RedirectToAction("AssignDuty", new { id = model.DutyId });
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }
        public ActionResult RemoveDuty(int DutyId, int AgentId)
        {
            var crt = objdb.AssignDuties.Where(x => x.AgentId == AgentId && x.DutyId == DutyId).SingleOrDefault();
            objdb.AssignDuties.Remove(crt);
            objdb.SaveChanges();
            Session["Success"] = "Remove Agent successfully!";
            return RedirectToAction("AssignDuty", new { id = DutyId });
        }
        #endregion Duty

        #region  Subject
        public ActionResult Subject()
        {
            SubjectModel model = new SubjectModel();
            model.SubjectList = new SubjectBAL { }.AllSubject();
            CustomMethods.ValidateRoles("Subject");
            return View(model);
        }
        public ActionResult AddEditSubject(int id = 0)
        {
            SubjectModel model = new SubjectModel();
            if (id != 0)
            {
                SubjectModel obj = new SubjectBAL { }.GetSubjectById(id);
                if (obj != null)
                {
                    model.SubjectId = obj.SubjectId;
                    model.Title = obj.Title;
                    model.Description = obj.Description;
                    model.Image = obj.Image;

                    model.CreatedBy = obj.CreatedBy;
                    model.IsActive = obj.IsActive;

                }

            }
            CustomMethods.ValidateRoles("Subject");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditSubject(SubjectModel model, HttpPostedFileBase file)
        {
            SubjectBAL catBLL = new SubjectBAL();
            string strFileName = "";
            string path = "";
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                if (file != null)
                {
                    strFileName = "SubjectImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/Subjects/" + strFileName);
                    file.SaveAs(path);
                }
                res = new SubjectBAL { }.AddEditSubject(new SubjectModel
                {
                    SubjectId = model.SubjectId,
                    Title = model.Title,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    Image = strFileName,

                    CreatedBy = UserId,

                });
                if (res != 0)
                {
                    if (model.SubjectId == 0)
                    {
                        Session["Success"] = "Subject added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Subject Updated successfully!";
                    }
                    return RedirectToAction("Subject");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }

        #endregion Subject

        #region  Award
        public ActionResult Award()
        {
            AwardModel model = new AwardModel();
            model.AwardList = new AwardBAL { }.AllAward();
            CustomMethods.ValidateRoles("Award");
            return View(model);
        }
        public ActionResult AddEditAward(int id = 0)
        {
            AwardModel model = new AwardModel();
            if (id != 0)
            {
                AwardModel obj = new AwardBAL { }.GetAwardById(id);
                if (obj != null)
                {
                    model.AwardId = obj.AwardId;
                    model.Title = obj.Title;
                    model.UserId = obj.UserId;

                }

            }
            var agents = new UserBAL { }.AllUser().Where(x => x.RoleId == 3 || x.RoleId == 4).ToList();
            if (agents != null)
            {
                model.GetType().GetProperty("ListUser").SetValue(model, agents.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }));
            }
            CustomMethods.ValidateRoles("Award");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditAward(AwardModel model, HttpPostedFileBase file)
        {
            AwardBAL catBLL = new AwardBAL();
            string strFileName = "";
            string path = "";
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                if (file != null)
                {
                    strFileName = "AwardImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/Awards/" + strFileName);
                    file.SaveAs(path);
                }
                res = new AwardBAL { }.AddEditAward(new AwardModel
                {
                    AwardId = model.AwardId,
                    Title = model.Title,


                    UserId = model.UserId,

                });
                if (res != 0)
                {
                    if (model.AwardId == 0)
                    {
                        Session["Success"] = "Award added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Award Updated successfully!";
                    }
                    return RedirectToAction("Award");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }

        #endregion Award


        #region  CompanyProfile
        public ActionResult CompanyProfile()
        {
            CompanyProfileModel model = new CompanyProfileModel();
            model.CompanyProfileList = new CompanyProfileBAL { }.AllCompanyProfile();
            CustomMethods.ValidateRoles("CompanyProfile");
            return View(model);
        }
        public ActionResult AddEditCompanyProfile(int id = 0)
        {
            CompanyProfileModel model = new CompanyProfileModel();
            if (id != 0)
            {
                CompanyProfileModel obj = new CompanyProfileBAL { }.GetCompanyProfileById(id);
                if (obj != null)
                {
                    model.ProfileId = obj.ProfileId;
                    model.Title = obj.Title;
                    model.Description = obj.Description;
                    model.Logo = obj.Logo;


                }

            }
            CustomMethods.ValidateRoles("CompanyProfile");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditCompanyProfile(CompanyProfileModel model, HttpPostedFileBase file)
        {
            CompanyProfileBAL catBLL = new CompanyProfileBAL();
            string strFileName = "";
            string path = "";
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                if (file != null)
                {
                    strFileName = "CompanyProfileImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/Profiles/" + strFileName);
                    file.SaveAs(path);
                }
                res = new CompanyProfileBAL { }.AddEditCompanyProfile(new CompanyProfileModel
                {
                    ProfileId = model.ProfileId,
                    Title = model.Title,
                    Description = model.Description,
                  
                    Logo = strFileName,


                });
                if (res != 0)
                {
                    if (model.ProfileId == 0)
                    {
                        Session["Success"] = "CompanyProfile added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "CompanyProfile Updated successfully!";
                    }
                    return RedirectToAction("CompanyProfile");
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }

        #endregion CompanyProfile

        #region  UserDocument
        public ActionResult Document()
        {        
            return View();
        }
        public ActionResult UserDocument()
        {
            UserDocumentModel model = new UserDocumentModel();
            Session["Type"] = "User";
            if (Session["UserId"].ToString() != null)
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //model.UserDocumentList = new UserDocumentBAL { }.AllUserDocument().Where(x => x.DocType == "User" && x.UserId == UserId).ToList();
                model.UserDocumentList = new UserDocumentBAL { }.AllUserDocument().Where(x => x.DocType == "User").ToList();

                CustomMethods.ValidateRoles("UserDocument");
            }
            else
            {
                return RedirectToAction("Index");
            }
           
            return View(model);
        }
        public ActionResult ManagerDocument()
        {
            UserDocumentModel model = new UserDocumentModel();
            Session["Type"] = "Manager";
            if (Session["UserId"].ToString() != null)
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                model.UserDocumentList = new UserDocumentBAL { }.AllUserDocument().Where(x => x.DocType == "Manager").ToList();
                //model.UserDocumentList = new UserDocumentBAL { }.AllUserDocument().Where(x => x.DocType == "Manager" && x.UserId == UserId).ToList();
                CustomMethods.ValidateRoles("UserDocument");
            }
            else
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }
        public ActionResult AddEditUserDocument(int id = 0)
        {
            UserDocumentModel model = new UserDocumentModel();
            model.DocType = Session["Type"].ToString();
            if (id != 0)
            {
                UserDocumentModel obj = new UserDocumentBAL { }.GetUserDocumentById(id);
                if (obj != null)
                {
                    model.UserDocumentId = obj.UserDocumentId;
                    model.DocumentName = obj.DocumentName;
                    model.UserId = obj.UserId;
                    model.Title = obj.Title;
                    model.Description = obj.Description;
                    model.DocType = obj.DocType;
                    model.DocumentExtension = obj.DocumentExtension;
                    model.IsActive = obj.IsActive;

                }
            }
       
            CustomMethods.ValidateRoles("UserDocument");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditUserDocument(UserDocumentModel model, HttpPostedFileBase file)
        {
            UserDocumentBAL catBLL = new UserDocumentBAL();
            string extn = "";

            string strFileName = ""; 
            string path = "";
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
               
                if (file != null)
                {
                    strFileName = "DocumentImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                    path = Server.MapPath("~/Images/UserDocuments/" + strFileName);
                    extn = System.IO.Path.GetExtension(path);
                    file.SaveAs(path);
                }

                res = new UserDocumentBAL { }.AddEditUserDocument(new UserDocumentModel
                {
                    UserDocumentId = model.UserDocumentId,
                    DocumentName = strFileName,
                    Title = model.Title,
                    Description = model.Description,
                    DocumentExtension=extn,
                    UserId = UserId,
                    DocType =model.DocType,
                    IsActive = model.IsActive,

                });
                if (res != 0)
                {
                    if (model.UserDocumentId == 0)
                    {
                        Session["Success"] = "Document added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Document Updated successfully!";
                    }
                    if (model.DocType == "User")
                    {
                        return RedirectToAction("UserDocument");
                    }
                    else {
                        return RedirectToAction("ManagerDocument");
                    }
              
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }


        public ActionResult ChangeUserDocumentStatus(long id)
        {
            bool Result = false;
            bool ChangeStatus = new UserDocumentBAL { }.ChangeDocumentStatus(id);
            if (ChangeStatus)
            {
                Result = true;
            }
          
            return RedirectToAction("UserDocument", "Home");
        }
        public ActionResult ChangeGenDocumentStatus(long id)
        {
            bool Result = false;
            bool ChangeStatus = new UserDocumentBAL { }.ChangeDocumentStatus(id);
            if (ChangeStatus)
            {
                Result = true;
            }

            return RedirectToAction("ManagerDocument", "Home");
        }
        #endregion UserDocument

        #region Roles

        public ActionResult Roles(int pid = 0)
        {
            Session["Active"] = "Roles";
            //Session["Script"] = "yes";
            int take = 10;
            int skip = take * pid;

            RoleModel model = new RoleModel();
            model.PageID = pid;
            model.Current = pid + 1;
            IEnumerable<RoleModel> roles = new List<RoleModel>();
            CustomMethods.ValidateRoles("Role");
            var roleslist = new RolesBAL { }.GetAllRoles(skip, take);

            double count = Convert.ToDouble(new RolesBAL { }.GetPageCount());
            var res = count / take;
            model.Pagecount = (int)Math.Ceiling(res);

            if (roleslist != null)
            {
                model.RolesList = roleslist.Select(x => new RoleModel
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    IsActive = Convert.ToBoolean(x.IsActive)
                }).ToList();
            }
            return View(model);
        }

        public ActionResult AddEditRoles(int id = 0)
        {
            Session["Active"] = "Roles";
            try
            {
                Session["Script"] = "No";
                CustomMethods.ValidateRoles("Role");
                if (!Convert.ToBoolean(Session["Add"]) && !Convert.ToBoolean(Session["Edit"]))
                    return View("ErrorPage", "Error");
                RoleModel model = new RoleModel();

                string str = Server.MapPath("~/RoleManagement.xml");
                var xml = XDocument.Load(str);
                var xmlpages = xml.Root.Elements("Page").Select(x => new
                {
                    id = x.Attribute("Id").Value,
                    name = x.Attribute("Name").Value,

                }).ToList();
                model.Pagecount = xmlpages.Count;

                if (id != 0)
                {
                    var res = new RolesBAL { }.GetRoleById(id);
                    if (res != null)
                    {
                        model.RoleId = res.RoleId;
                        model.RoleName = res.RoleName;
                        model.IsActive = Convert.ToBoolean(res.IsActive);

                        model.Rolemanages = res.Rolemanages.Select(x => new RolemanagementModel
                        {
                            Add = Convert.ToBoolean(x.Add),
                            Edit = Convert.ToBoolean(x.Edit),
                            Delete = Convert.ToBoolean(x.Delete),
                            View = Convert.ToBoolean(x.View),
                            RoleID = Convert.ToInt32(x.RoleID),
                            PageID = Convert.ToInt32(x.PageID),
                            PageName = xmlpages.Where(z => Convert.ToInt32(z.id) == x.PageID).Select(z => z.name).SingleOrDefault()
                        }).ToList();
                        if (model.Rolemanages.Count == 0)
                        {
                            model.Rolemanages = xmlpages.Select(x => new RolemanagementModel
                            {
                                Add = false,
                                Delete = false,
                                View = false,
                                Edit = false,
                                PageID = Convert.ToInt32(x.id),
                                PageName = x.name
                            }).ToList();
                        }

                    }



                }
                else
                {
                    model.Rolemanages = xmlpages.Select(x => new RolemanagementModel
                    {
                        Add = false,
                        Delete = false,
                        View = false,
                        Edit = false,
                        PageID = Convert.ToInt32(x.id),
                        PageName = x.name
                    }).ToList();
                }



                return View(model);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        [HttpPost]
        public ActionResult AddEditRoles(RoleModel model, FormCollection Pages)
        {
            try
            {
                if (model.RoleId == 0)
                {
                    if (!Convert.ToBoolean(Session["Add"]))
                        return View("ErrorPage");
                }
                else
                {
                    if (!Convert.ToBoolean(Session["Edit"]))
                        return View("ErrorPage");
                }

                RoleModel objrole = new RoleModel();

                for (int i = 1; i <= model.Pagecount; i++)
                {
                    RolemanagementModel obj = new RolemanagementModel();

                    var add = Pages["page_Add" + i];
                    var edit = Pages["page_edit" + i];
                    var delete = Pages["page_delete" + i];
                    var read = Pages["page_read" + i];

                    obj.Add = add == null ? false : true;
                    obj.Edit = edit == null ? false : true;
                    obj.Delete = delete == null ? false : true;
                    obj.View = read == null ? false : true;
                    //obj.PageID = Convert.ToInt32(Pages["hd_page" + i]);
                    int ss = Convert.ToInt32(Pages["hd_page"]);
                    obj.PageID = Convert.ToInt32(ss + i);
                    objrole.Rolemanages.Add(obj);
                }

                if (ModelState.IsValid)
                {
                    long res = new RolesBAL { }.AddEditRole(new RoleModel
                    {
                        RoleId = model.RoleId,
                        RoleName = model.RoleName,
                        IsActive = model.IsActive,
                        Rolemanages = objrole.Rolemanages
                    });
                    if (res != 0)
                    {
                        if (model.RoleId == 0)
                        {
                            Session["Success"] = "Roles added successfully!";
                        }
                        else
                        {
                            Session["Success"] = "Roles Updated successfully!";
                        }
                        return RedirectToAction("Roles");
                    }

                    else
                    {
                        long result = new RolesBAL { }.AddEditRole(new RoleModel
                        {
                            RoleId = model.RoleId,
                            RoleName = model.RoleName,
                            IsActive = model.IsActive,
                            Rolemanages = objrole.Rolemanages
                        });
                        if (result != 0)
                        {
                            Session["Success"] = "Successfully Updated The Record";
                            return RedirectToAction("Roles");
                        }
                    }
                }
                Session["Error"] = "An Error has occured";
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }

        public ActionResult ViewRoles(int id = 0)
        {
            Session["Active"] = "Roles";
            try
            {
                CustomMethods.ValidateRoles("Role");
                if (!Convert.ToBoolean(Session["Add"]) && !Convert.ToBoolean(Session["Edit"]))
                    return View("ErrorPage", "Error");
                RoleModel model = new RoleModel();

                string str = Server.MapPath("~/Areas/Admin/RoleManagement.xml");
                var xml = XDocument.Load(str);
                var xmlpages = xml.Root.Elements("Page").Select(x => new
                {
                    id = x.Attribute("Id").Value,
                    name = x.Attribute("Name").Value,

                }).ToList();
                model.Pagecount = xmlpages.Count;

                if (id != 0)
                {
                    var res = new RolesBAL { }.GetRoleById(id);
                    if (res != null)
                    {
                        model.RoleId = res.RoleId;
                        model.RoleName = res.RoleName;
                        model.IsActive = Convert.ToBoolean(res.IsActive);

                        model.Rolemanages = res.Rolemanages.Select(x => new RolemanagementModel
                        {
                            Add = Convert.ToBoolean(x.Add),
                            Edit = Convert.ToBoolean(x.Edit),
                            Delete = Convert.ToBoolean(x.Delete),
                            View = Convert.ToBoolean(x.View),
                            RoleID = Convert.ToInt32(x.RoleID),
                            PageID = Convert.ToInt32(x.PageID),
                            PageName = xmlpages.Where(z => Convert.ToInt32(z.id) == x.PageID).Select(z => z.name).SingleOrDefault()
                        }).ToList();
                        if (model.Rolemanages.Count == 0)
                        {
                            model.Rolemanages = xmlpages.Select(x => new RolemanagementModel
                            {
                                Add = false,
                                Delete = false,
                                View = false,
                                Edit = false,
                                PageID = Convert.ToInt32(x.id),
                                PageName = x.name
                            }).ToList();
                        }

                    }



                }
                else
                {
                    model.Rolemanages = xmlpages.Select(x => new RolemanagementModel
                    {
                        Add = false,
                        Delete = false,
                        View = false,
                        Edit = false,
                        PageID = Convert.ToInt32(x.id),
                        PageName = x.name
                    }).ToList();
                }



                return View(model);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        public JsonResult ChangeRolesStatus(int id)
        {
            bool Result = false;
            bool ChangeStatus = new RolesBAL { }.ChangeStatus(id);
            if (ChangeStatus)
            {
                Result = true;
            }
            return Json(Result);
        }

        public ActionResult DeleteRoles(int id)
        {

            try
            {
                var Model = objdb.Roles.Where(x => x.RoleId == id).FirstOrDefault();

                if (Model == null)
                {
                    return HttpNotFound();
                }

                objdb.Roles.Remove(Model);
                objdb.SaveChanges();
                return RedirectToAction("Roles");
            }
            catch (Exception ex)
            {
                Session["Rls"] = "Roles";
                return RedirectToAction("Roles");

            }

        }

        #endregion

        #region  ManagerdistributeAmount
        public ActionResult ManagerdistributeAmount()
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountListbyid(UserId);
            CustomMethods.ValidateRoles("Distribute Amount");
            return View(model);
        }
        public ActionResult AddEditManagerdistributeAmount(int IID = 0)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            if (UserId != 0)
            {
                ManagerdistributeAmountModel modela = new ManagerdistributeAmountBAL { }.GetAgentbyid(IID); 
                if(modela!=null)
                {
                    model.ID = 0;
                    model.AgentName = modela.AgentName;
                    model.AgentId = modela.AgentId;
                    model.ManagerId = modela.ManagerId;
                }
                
                
            }
            //CustomMethods.ValidateRoles("UserDocument");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditManagerdistributeAmount(ManagerdistributeAmountModel model)
        {
            UserDocumentBAL catBLL = new UserDocumentBAL();
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                long res1 = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                {
                    ID = model.ID,
                    AgentId = model.AgentId,
                    ManagerId = model.ManagerId,
                    Amount = model.Amount,
                    Date = Date

                });         
                if (res != 0)
                {
                    if (model.ID == 0)
                    {
                        Session["Success"] = "Amount added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Amount Updated successfully!";
                    }
                        return RedirectToAction("ManagerdistributeAmount");
                }
                return RedirectToAction("ManagerdistributeAmount");
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                throw;
            }
        }

        public ActionResult AddEditManagerdistributeListAmount(int IID = 0)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            if (UserId != 0)
            {
                ManagerdistributeAmountModel modela = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountById(IID);
                if (modela != null)
                {
                    model.ID = modela.ID;
                    model.AgentName = modela.AgentName;
                    model.AgentId = modela.AgentId;
                    model.ManagerId = modela.ManagerId;
                    model.Amount = modela.Amount;
                }
            }
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditManagerdistributeListAmount(ManagerdistributeAmountModel model)
        {
            UserDocumentBAL catBLL = new UserDocumentBAL();
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                long res1 = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                {
                    ID = model.ID,
                    AgentId = model.AgentId,
                    ManagerId = model.ManagerId,
                    Amount = model.Amount,
                    Date = Date

                });
                if (res != 0)
                {
                    if (model.ID == 0)
                    {
                        Session["Success"] = "Amount added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Amount Updated successfully!";
                    }
                    return RedirectToAction("ManagerdistributeAmount");
                }
                return RedirectToAction("DistributionList");
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                throw;
            }
        }
        public ActionResult DistributionList()
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountListbyid2(UserId).Where(x => x.IsActive == false).ToList();
            if (Convert.ToInt32(Session["RoleId"]) == 4)
            {
                model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountListbyid2(UserId).Where(x => x.IsActive == false).ToList();
            }
            else
            {
                model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList().Where(x=>x.IsActive==false).ToList();
            }
            CustomMethods.ValidateRoles("Distribute Amount");
            return View(model);
        }

        public ActionResult AgentTotalDistributionList()
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            
            model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountListbyid2(UserId).Where(x => x.IsActive == false).ToList();
            if (Convert.ToInt32(Session["RoleId"]) == 4)
            {
                var list = objdb.ManagerdistributeAmounts.Where(x => x.IsActive == true & x.ManagerId == UserId).GroupBy(x => x.AgentId).Select(x => new
                {
                    ID = x.FirstOrDefault().ID,
                    Name = x.FirstOrDefault().TblUser.Name,
                    Amount = x.Sum(c => c.Amount)
                }).ToList();
                model.ManagerdistributeAmountList = list.Select(x => new ManagerdistributeAmountModel
                {
                    ID = x.ID,
                    AgentName = x.Name,
                    Amount = x.Amount
                }).ToList();
            }
            else
            {
                var list = objdb.ManagerdistributeAmounts.Where(x => x.IsActive == true ).GroupBy(x => x.AgentId).Select(x => new
                {
                    ID = x.FirstOrDefault().ID,
                    Name = x.FirstOrDefault().TblUser.Name,
                    Amount = x.Sum(c => c.Amount)
                }).ToList();
                model.ManagerdistributeAmountList = list.Select(x => new ManagerdistributeAmountModel
                {
                    ID = x.ID,
                    AgentName = x.Name,
                    Amount = x.Amount
                }).ToList();
            }
            CustomMethods.ValidateRoles("Distribute Amount");
            return View(model);
        }
        public ActionResult ExportData()
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            var list = objdb.TblUsers.Where(x => x.ManagerID == UserId).Select(x => new 
            {
                AgentId = x.UserId,
                AgentName = x.Name,
                ManagerId = x.ManagerID,
                ManagerName = x.Name,
                Amount = 00,
            }).OrderByDescending(x => x.AgentId).ToList();
            GridView grdProjectTasks = new GridView();
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook Wb = xla.Workbooks.Add(Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xla.ActiveSheet;
            xla.Visible = true;
            ws.Cells[1, 1]="Agent Id";
            ws.Cells[1, 2] = "Agent Name";
            ws.Cells[1, 3] = "Manager Id";
            ws.Cells[1, 4] = "Manager Name";
            ws.Cells[1, 5] = "Amount";
            int i = 2;
            foreach (var item in list)
            {
                ws.Cells[i, 1] = item.AgentId;
                ws.Cells[i, 2] = item.AgentName;
                ws.Cells[i, 3] = item.ManagerId;
                ws.Cells[i, 4] = item.ManagerName;
                ws.Cells[i, 5] = item.Amount;
                i++;
            }
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=AgentList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(list, Response.Output);
            Response.End();

            Response.ClearContent();
            Response.ClearHeaders();
            grdProjectTasks.DataSource = list;
            grdProjectTasks.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "AgentList" + ".xls"));
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw1 = new StringWriter();
            HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
            grdProjectTasks.AllowPaging = false;
            grdProjectTasks.RenderControl(htw1);
            Response.Write(sw1.ToString());
            Response.End();

            return RedirectToAction("ManagerdistributeAmount");
        }

        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult ImportAgentList(List<HttpPostedFileBase> file)
        {
            try
            {

                int _UserId = Convert.ToInt32(Session["AdminUserId"]);
                string strFileName1 = ""; string path1 = "";
                Random rnd = new Random();  long res = 0;
                DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
                foreach (var item1 in file)
                {
                    if (item1 != null)
                    {
                        strFileName1 = "ExcelAgent" + rnd.Next(100, 100000000) + "." + item1.FileName.Split('.')[1].ToString();
                        path1 = Server.MapPath("~/Images/AgentExcel/" + strFileName1);
                        item1.SaveAs(path1);
                        string pathToExcelFile = path1;
                        string sheetName = "Sheet1";
                        var excelFile = new ExcelQueryFactory(pathToExcelFile);
                        var StudentData = from a in excelFile.Worksheet(sheetName) select a;
                        foreach (var Student in StudentData)
                        {
 
                            if (Convert.ToDecimal(Student[4]) != 0)
                            {
                                long AgentId = Convert.ToInt32(Student[0]);
                                long ManagerId = Convert.ToInt32(Student[2]);
                                decimal Amount = Convert.ToDecimal(Student[4]);

                                if (Amount != 0)
                                {
                                    res = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                                    {
                                        ID = 0,
                                        AgentId = AgentId,
                                        ManagerId = ManagerId,
                                        Amount = Amount,
                                        Date = Date

                                    });
                                }                                      
                            }
                        }
                    }

                    System.IO.File.Delete(path1);
                }
                if (res != 0)
                {
                    //TempData["msg"] = "<script>alert('Student has been Added Successfully');</script>";
                    Session["Success"] = "Amount added successfully";
                    return RedirectToAction("ManagerdistributeAmount");
                }
                Session["Error"] = "An Error has occured";
                return RedirectToAction("ManagerdistributeAmount");
            }
            catch (Exception)
            {
                //Session["Error"] = "An Error has occured";
                //return View(model);
                //throw;
            }
            return View();
        }

        #endregion ManagerdistributeAmount

        #region SalesAmount
        public ActionResult SalesAmount()
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            if (Session["RoleId"].ToString() == "4")  //manager
            {
                var list = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList().Where(x=>x.ManagerId==UserId && x.AgentId!=UserId).ToList();
                var lst = list.Select(x => new 
                {
                    ManagerId = x.ManagerId,
                    ManagerName=x.Manager,
                    Date = x.Date,
                    Count=list.Where(y=>y.ManagerId==UserId && y.Date==x.Date).ToList().Count(),
                    IsActive=x.IsActive,
                }).Distinct().ToList();
                model.ManagerdistributeAmountList = lst.Select(x => new ManagerdistributeAmountModel
                {
                    ManagerId = x.ManagerId,
                    Manager = x.ManagerName,
                    Date = x.Date,
                    Count = x.Count,
                    IsActive = x.IsActive,
                }).Distinct().ToList();
            }
            else
            {
                var list = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList().ToList();
                var lst = list.Select(x => new
                {
                    //ManagerId = x.ManagerId,
                    //ManagerName = x.Manager,
                    Date = x.Date,
                    //Count = list.Where(y => y.Date == x.Date).ToList().Count(),
                    IsActive = x.IsActive,
                }).Distinct().ToList();
                model.ManagerdistributeAmountList = lst.Select(x => new ManagerdistributeAmountModel
                {
                    //ManagerId = x.ManagerId,
                    //Manager = x.ManagerName,
                    Date = x.Date,
                    Count = list.Where(y => y.Date == x.Date).ToList().Count(),
                    IsActive = x.IsActive,
                }).Distinct().ToList();
            }
            //model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountListbyid(UserId);
            CustomMethods.ValidateRoles("Distribute Amount");
            return View(model);
        }

        public ActionResult AddEditSubmissionAmount(int ManagerId=0,string Date="")
        {
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            if (Session["RoleId"].ToString() == "4")  //manager
            {
                if (Date != "")
                {
                    DateTime dt = Convert.ToDateTime(Date);
                    model.DateStr = dt.ToString("yyyy-MM-dd");
                    model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountListbyid2(UserId).Where(x => x.Date.Date == dt.Date && x.AgentId!=UserId).ToList();
                }
                else  //add new
                {
                    model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountListbyid(UserId).ToList();
                }
            }
            else if (Session["RoleId"].ToString() != "4")  //admin
            {
                if (Date != "")
                {
                    DateTime dt = Convert.ToDateTime(Date);
                    model.DateStr = dt.ToString("yyyy-MM-dd");
                    model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountAdmin().Where(x => x.Date.Date == dt.Date).ToList();

                    var list1 = objdb.TblUsers.Where(x => x.RoleId == 3 || x.RoleId == 4).Select(x => new
                    {
                        ID = 0,
                        ManagerId = x.ManagerID,
                        Code = x.AgentCode,
                       
                        AgentId = x.UserId,
                        AgentName = x.Name,
                        Amount = 0,
                    }).ToList();
                    var list2 = model.ManagerdistributeAmountList.Select(x => new
                    {
                        ID = x.ID,
                        ManagerId = x.ManagerId,
                        Code = x.Code,

                        AgentId = x.AgentId,
                        AgentName = x.AgentName,
                        Amount = x.Amount,
                    }).ToList();



                    var joinedList = (from s in list1
                                      join cs in list2 on s.AgentId equals cs.AgentId
                                      into studentInfo
                                      from students in studentInfo.DefaultIfEmpty()
                                      select new
                                      {
                                          //s.ID,
                                          s.ManagerId,
                                          s.AgentId,
                                          s.AgentName,
                                          s.Code,
                                          Amount = students == null ? 0 : students.Amount,
                                          ID = students == null ? 0 : students.ID,
                                      }).Distinct().ToList();

                    model.ManagerdistributeAmountList = joinedList.Select(x => new ManagerdistributeAmountModel
                    {
                        ID = x.ID,
                        ManagerId = x.ManagerId,

                        AgentId = x.AgentId,
                        AgentName = x.AgentName,
                        Amount = x.Amount,
                        Code = x.Code,
                        Manager=objdb.TblUsers.Where(y=>y.UserId==x.AgentId).Select(y=>y.Name).SingleOrDefault(),
                    }).ToList();
                }
                else  //add new
                {
                    model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountadminBydef().ToList();
                }
            }




            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditSubmissionAmount(FormCollection form,ManagerdistributeAmountModel model)
        {
            UserDocumentBAL catBLL = new UserDocumentBAL();
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            model.Date = Convert.ToDateTime(model.DateStr);
            int chk = 0;
            try
            {
                long res = 0;
                string[] ids = Request.Form.GetValues("id");
                string[] Managers = Request.Form.GetValues("Manager");
                string[] Agents = Request.Form.GetValues("Agent");
                string[] Amounts = Request.Form.GetValues("Amount");
                long i = 0;
                if (Session["RoleId"].ToString() == "4")
                {
                    

                    foreach (var item1 in Amounts)
                    {

                        long iid = 0;
                        if (ids[i].Trim() != "")
                        {
                            iid = Convert.ToInt64(ids[i].Trim());
                        }
                        if (iid == 0)
                        {
                            if (i == 0)
                            {
                                chk = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList().Where(x => x.ManagerId == UserId && x.Date.Date == model.Date.Date).ToList().Count();
                                if (chk > 0)
                                {
                                    Session["warning"] = "Sorry You can not add on same date Multiples record!";
                                    return RedirectToAction("AddEditSubmissionAmount");
                                }
                                else
                                {
                                    res = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                                    {
                                        ID = iid,
                                        AgentId = Convert.ToInt64(Agents[i].Trim()),
                                        ManagerId = Convert.ToInt64(Managers[i].Trim()),
                                        Amount = Convert.ToDecimal(Amounts[i].Trim()),
                                        Date = Convert.ToDateTime(model.DateStr),

                                    });
                                }
                            }
                            else
                            {
                                res = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                                {
                                    ID = iid,
                                    AgentId = Convert.ToInt64(Agents[i].Trim()),
                                    ManagerId = Convert.ToInt64(Managers[i].Trim()),
                                    Amount = Convert.ToDecimal(Amounts[i].Trim()),
                                    Date = Convert.ToDateTime(model.DateStr),

                                });
                            }

                        }
                        else
                        {
                            res = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                            {
                                ID = iid,
                                AgentId = Convert.ToInt64(Agents[i].Trim()),
                                ManagerId = Convert.ToInt64(Managers[i].Trim()),
                                Amount = Convert.ToDecimal(Amounts[i].Trim()),
                                Date = Convert.ToDateTime(model.DateStr),

                            });
                        }



                        i++;
                    }
                    var st = objdb.ManagerdistributeAmounts.ToList();
                    var some = st.Where(x => x.Date.Date == model.Date.Date).ToList();
                    some.ForEach(a => a.IsActive = false);
                    objdb.SaveChanges();
                }
                else 
                {
                    foreach (var item1 in Amounts)
                    {
                        long AgntId = Convert.ToInt64(Agents[i].Trim());
                        long ManagerId = 0;
                        var chkagnOrMng = objdb.TblUsers.Where(x => x.UserId == AgntId).SingleOrDefault();
                        if (chkagnOrMng.ManagerID == null)
                        {
                            ManagerId = chkagnOrMng.UserId;
                        }
                        else {
                            ManagerId = Convert.ToInt32(chkagnOrMng.ManagerID);
                        }

                        long iid = 0;
                        if (ids[i].Trim() != "")
                        {
                            iid = Convert.ToInt64(ids[i].Trim());
                        }
                        if (iid == 0)
                        {
                            if (i == 0)
                            {


                                chk = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList().Where(x => x.ManagerId == ManagerId && x.Date.Date == model.Date.Date).ToList().Count();
                                if (chk > 0)
                                {
                                    Session["warning"] = "Sorry You can not add on same date Multiples record!";
                                    return RedirectToAction("AddEditSubmissionAmount");
                                }
                                else
                                {
                                    res = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                                    {
                                        ID = iid,
                                        AgentId = Convert.ToInt64(Agents[i].Trim()),
                                        ManagerId = ManagerId,
                                        Amount = Convert.ToDecimal(Amounts[i].Trim()),
                                        Date = Convert.ToDateTime(model.DateStr),

                                    });
                                }
                            }
                            else
                            {
                                res = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                                {
                                    ID = iid,
                                    AgentId = Convert.ToInt64(Agents[i].Trim()),
                                    ManagerId=ManagerId,
                                    //ManagerId = Convert.ToInt64(Managers[i].Trim()),
                                    Amount = Convert.ToDecimal(Amounts[i].Trim()),
                                    Date = Convert.ToDateTime(model.DateStr),

                                });
                            }

                        }
                        else
                        {
                            res = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                            {
                                ID = iid,
                                AgentId = Convert.ToInt64(Agents[i].Trim()),
                                ManagerId = ManagerId,
                                Amount = Convert.ToDecimal(Amounts[i].Trim()),
                                Date = Convert.ToDateTime(model.DateStr),

                            });
                        }



                        i++;
                    }

                  
                }
                if (res != 0)
                {

                    Session["Success"] = "Data Save successfully!";
                    return RedirectToAction("SalesAmount");
                }
                return RedirectToAction("SalesAmount");
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                throw;
            }
        }

        public ActionResult PublishSubmission(string Date)
        {
            DateTime dt = Convert.ToDateTime(Date);
            bool Result = false;
            var list = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountAdmin().Where(x => x.Date.Date == dt.Date).ToList();
            foreach (var item in list)
            {
                bool ChangeStatus = new ManagerdistributeAmountBAL { }.ChangeManagerdistributeAmountsStatus(Convert.ToInt32(item.ID));
                if (ChangeStatus)
                {
                    Result = true;
                }
            }
            Session["Success"] = "Published Successfully!";

            return RedirectToAction("SalesAmount", "Home");
        }
        public ActionResult UnPublishSubmission(string Date)
        {
            DateTime dt = Convert.ToDateTime(Date);
            bool Result = false;
            var list = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountAdmin().Where(x => x.Date.Date == dt.Date).ToList();
            foreach (var item in list)
            {
                bool ChangeStatus = new ManagerdistributeAmountBAL { }.ChangeManagerdistributeAmountsStatus(Convert.ToInt32(item.ID));
                if (ChangeStatus)
                {
                    Result = true;
                }
            }
            Session["Success"] = "Published Successfully!";

            return RedirectToAction("SalesAmount", "Home");
        }
        #endregion 

        #region  Chart
        public ActionResult Chart()
        {
            //long UserId = Convert.ToInt32(Session["UserId"].ToString());
            //ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            //model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList();
            //CustomMethods.ValidateRoles("Distribute Amount");
            return View();
        }



        public JsonResult GetChartList()
        {
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            try
            {
                var list1 = objdb.ManagerdistributeAmounts.Where(x => x.Date.Month == Date.Month).Select(x => new { 
                ID = x.ID,
                Name = x.TblUser.Name,
                AgentId = x.AgentId,
                ManagerId = x.ManagerId,
                Amount = x.Amount,
                IsActive = x.IsActive
                }).ToList();
                var list3 = list1.Where(x => x.IsActive == true).Select(x => new
                {
                    ID = x.ID,
                    Name = x.Name,
                    AgentId = x.AgentId,
                    ManagerId = x.ManagerId,
                    Amount = x.Amount,
                    IsActive = x.IsActive
                }).ToList();

                if (Session["RoleId"].ToString() == "4")
                {
                    var list = list3.Where(x => x.ManagerId == UserId).GroupBy(x => x.AgentId).Select(x => new
                    {
                        ID = x.FirstOrDefault().ID,
                        Name = x.FirstOrDefault().Name,
                        Amount = x.Sum(c => c.Amount)
                    }).ToList();
                    return Json(list);
                }
                else
                {
                    var list = list3.Where(x => x.IsActive == true).GroupBy(x => x.AgentId).Select(x => new
                    {
                        ID = x.FirstOrDefault().ID,
                        Name = x.FirstOrDefault().Name,
                        Amount = x.Sum(c => c.Amount)
                    }).ToList();
                    return Json(list);
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult GetChartListByDate(string fromdate, string ToDate)
        {
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            DateTime FromDate = Convert.ToDateTime(fromdate);
            DateTime Todate = Convert.ToDateTime(ToDate);
            try
            {

                var list1 = objdb.ManagerdistributeAmounts.Where(x => x.Date >= FromDate & x.Date <= Todate).Select(x => new
                {
                    ID = x.ID,
                    Name = x.TblUser.Name,
                    AgentId = x.AgentId,
                    ManagerId = x.ManagerId,
                    Amount = x.Amount,
                    IsActive = x.IsActive
                }).ToList();
                var list3 = list1.Where(x => x.IsActive == true).Select(x => new
                {
                    ID = x.ID,
                    Name = x.Name,
                    AgentId = x.AgentId,
                    ManagerId = x.ManagerId,
                    Amount = x.Amount,
                    IsActive = x.IsActive
                }).ToList();
                if (Session["RoleId"].ToString() == "4")
                {
                    var list = list3.Where(x => x.ManagerId == UserId & x.IsActive == true).GroupBy(x => x.AgentId).Select(x => new
                    {
                        ID = x.FirstOrDefault().ID,
                        Name = x.FirstOrDefault().Name,
                        Amount = x.Sum(c => c.Amount)
                    }).ToList();
                    return Json(list);
                }
                else 
                {
                    var list = list3.Where(x => x.IsActive == true).GroupBy(x => x.AgentId).Select(x => new
                    {
                        ID = x.FirstOrDefault().ID,
                        Name = x.FirstOrDefault().Name,
                        Amount = x.Sum(c => c.Amount)
                    }).ToList();
                    return Json(list);
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DailySubmissionChart(string FromDate = "")
        {
            ViewBag.chart = "Chart";
            DailySubmissionChartModel model = new DailySubmissionChartModel();
            var dChart = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList();
            var list1 = objdb.ManagerdistributeAmounts.Where(x=>x.IsActive==true).Select(x => new
            {
                ID = x.ID,
                Date=x.Date,
                Name = x.TblUser.Name,
                AgentId = x.AgentId,
                ManagerId = x.ManagerId,
                Amount = x.Amount,
                IsActive = x.IsActive
            }).ToList();

            var list2 = objdb.ManagerdistributeAmounts.Select(x => new
            {
                AgentId = x.AgentId,
                Name = x.TblUser.Name,
            }).Distinct().ToList();

            if (FromDate == "")
            {

                DateTime dt = DateTime.Now;
                //dt = dt.AddDays(-1);
                var list = list2.Select(x => new
                {
                    AgentName = x.Name,
                    //Date = x.Date,
                    Day1 = list1.Where(y => y.Date.Date < dt.Date && y.AgentId == x.AgentId && y.Date.Month == dt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.Date.Date < dt.Date && y.AgentId == x.AgentId && y.Date.Month == dt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Date == dt.Date && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.Date.Date == dt.Date && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                model.DailySubmissionChartList = list.Where(x => x.Day1 != 0 && x.Day != 0).Select(x => new DailySubmissionChartModel
                {
                    state = x.AgentName,
                    Day1 = x.Day1,
                    Day = x.Day,
                }).Distinct().ToList();
            }
            else 
            {


                model.Date = FromDate;
                DateTime Fromdt = Convert.ToDateTime(FromDate);
                //DateTime Todt = Convert.ToDateTime(ToDate);
                DateTime Dtless = Fromdt.AddDays(-1);
             
                //var list = list1.Where(x => x.Date.Month == Fromdt.Month).Select(x => new
                var list = list2.Select(x => new
                {
                    AgentName = x.Name,
                    //Date = x.Date,
                    Day1 = list1.Where(y => y.AgentId == x.AgentId && y.Date.Date < Fromdt.Date && y.Date.Month == Fromdt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.AgentId == x.AgentId && y.Date.Date < Fromdt.Date && y.Date.Month == Fromdt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Date == Fromdt.Date && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.Date.Date == Fromdt.Date && y.AgentId == x.AgentId ).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                model.DailySubmissionChartList = list.Where(x => x.Day1 != 0 && x.Day!=0).Select(x => new DailySubmissionChartModel
                {
                    state = x.AgentName,
                    Day1 = x.Day1,
                    Day = x.Day,
                }).Distinct().ToList();
            }

          
            return View(model);
        }


        public ActionResult MTDSubmissionChart(string FromDate = "")
        {
            ViewBag.chart = "Chart";
            DailySubmissionChartModel model = new DailySubmissionChartModel();
            var dChart = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList();
            var list1 = objdb.ManagerdistributeAmounts.Select(x => new
            {
                ID = x.ID,
                Date = x.Date,
                Name = x.TblUser.Name,
                AgentId = x.AgentId,
                ManagerId = x.ManagerId,
                Amount = x.Amount,
                IsActive = x.IsActive
            }).ToList();
            if (FromDate == "")
            {
                DateTime Fromdt = DateTime.Now;
                DateTime Dtless = Fromdt.AddMonths(-1);
                //dt = dt.AddDays(-1);
                var list = list1.Where(x => x.Date.Year == Fromdt.Year).Select(x => new
                {
                    AgentName = x.Name,
                    //Date = x.Date,
                    Day1 = list1.Where(y => y.AgentId == x.AgentId && x.Date.Month < Fromdt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Month == Fromdt.Month && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                model.DailySubmissionChartList = list.Where(x => x.Day1 != null).Select(x => new DailySubmissionChartModel
                {
                    state = x.AgentName,
                    Day1 = x.Day1,
                    Day = x.Day,
                }).Distinct().ToList();
            }
            else
            {
                model.Date = FromDate;
                DateTime Fromdt = Convert.ToDateTime(FromDate);

                DateTime Dtless = Fromdt.AddMonths(-1);

                var list = list1.Where(x => x.Date.Year == Fromdt.Year).Select(x => new
                {
                    AgentName = x.Name,
                    //Date = x.Date,
                    Day1 = list1.Where(y => y.AgentId == x.AgentId && y.Date.Month < Fromdt.Month && y.Date.Year == Fromdt.Year).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault()
                    == null ? 0 : list1.Where(y => y.AgentId == x.AgentId && y.Date.Month < Fromdt.Month && y.Date.Year == Fromdt.Year).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Month == Fromdt.Month && y.AgentId == x.AgentId && y.Date.Year == Fromdt.Year).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault()==null
                    ? 0 : list1.Where(y => y.Date.Month == Fromdt.Month && y.AgentId == x.AgentId && y.Date.Year == Fromdt.Year).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                model.DailySubmissionChartList = list.Where(x => x.Day1 != null).Select(x => new DailySubmissionChartModel
                {
                    state = x.AgentName,
                    Day1 = x.Day1,
                    Day = x.Day,
                }).Distinct().ToList();
            }


            return View(model);
        }

        public ActionResult MTDInceptionChart(string FromDate = "")
        {
            ViewBag.chart = "Chart";
            InceptionModel model = new InceptionModel();
            var dChart = new InceptionBAL { }.AllInception();
            var list1 = objdb.Inceptions.Select(x => new
            {
                InceptionId = x.InceptionId,
                Month = x.MonthFrom,
                Year = x.YearFrom,
                AgentId = x.AgentId,
                Name= x.TblUser.Name,
                MTD_WAPI = x.MTD_WAPI,
                YTD_WAPI = x.YTD_WAPI
            }).ToList();
            if (FromDate == "")
            {
                DateTime Fromdt = DateTime.Now;
                var list = list1.Where(x => x.Year== Fromdt.Year).Select(x => new
                {
                    AgentName = x.Name,
                    //Date = x.Date,
                    Year = list1.Where(y => y.AgentId == x.AgentId && x.Year == Fromdt.Year && y.Month < Fromdt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.YTD_WAPI)).SingleOrDefault() == null ? 0 : list1.Where(y => y.AgentId == x.AgentId && x.Year == Fromdt.Year && y.Month < Fromdt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.YTD_WAPI)).SingleOrDefault(),
                    Month = list1.Where(y => y.Month == Fromdt.Month && x.Year == Fromdt.Year && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.MTD_WAPI)).SingleOrDefault() == null ? 0 : list1.Where(y => y.Month == Fromdt.Month && x.Year == Fromdt.Year && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.MTD_WAPI)).SingleOrDefault(),
                }).Distinct().ToList();

                model.DailySubmissionChartList = list.Where(x => x.Year != null).Select(x => new DailySubmissionChartModel
                {
                    state = x.AgentName,
                    Year = x.Year,
                    //Year = Convert.ToInt32(x.Year) - Convert.ToInt32(x.Month),
                    Month = x.Month,
                }).Distinct().ToList();
            }
            else
            {
                model.Date = FromDate;
                DateTime Fromdt = Convert.ToDateTime(FromDate);

                var list = list1.Where(x => x.Year == Fromdt.Year).Select(x => new
                {
                    AgentName = x.Name,
                    //Date = x.Date,
                    Year = list1.Where(y => y.AgentId == x.AgentId && x.Year == Fromdt.Year && y.Month < Fromdt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.YTD_WAPI)).SingleOrDefault() == null ? 0 : list1.Where(y => y.AgentId == x.AgentId && x.Year == Fromdt.Year && y.Month < Fromdt.Month).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.YTD_WAPI)).SingleOrDefault(),
                    Month = list1.Where(y => y.Month == Fromdt.Month && x.Year == Fromdt.Year && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.MTD_WAPI)).SingleOrDefault() == null ? 0 : list1.Where(y => y.Month == Fromdt.Month && x.Year == Fromdt.Year && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.MTD_WAPI)).SingleOrDefault(),
                }).Distinct().ToList();

                model.DailySubmissionChartList = list.Where(x => x.Year != null).Select(x => new DailySubmissionChartModel
                {
                    state = x.AgentName,
                    //Year = Convert.ToInt32(x.Year) - Convert.ToInt32(x.Month),
                    Year=x.Year,
                    Month = x.Month,
                }).Distinct().ToList();
            }


            return View(model);
        }

        public ActionResult YTDTargetChart(int AgentId=0)
        {
            ViewBag.chart = "Chart";
            InceptionModel model = new InceptionModel();
            model.AgentId = AgentId;

            var usrlist = new UserBAL { }.AllUser().Where(x => x.RoleId == 3).ToList();
            if (usrlist != null)
            {
                model.GetType().GetProperty("ListAgent").SetValue(model, usrlist.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }));
            }
            if (AgentId > 0)
            {



                var Ince = objdb.Inceptions.Where(x=>x.AgentId==AgentId).GroupBy(x => x.AgentId).Select(x => new
                {
                    Inception = x.Sum(c => c.YTD_WAPI) == null ? 0 : x.Sum(c => c.YTD_WAPI),
                }).Distinct().SingleOrDefault();
                if (Ince != null)
                {
                    ViewBag.Inception = Ince.Inception;
                }
                else 
                {
                    ViewBag.Inception = 0;
                }

                var Tget = objdb.TblUsers.Where(x => x.UserId == AgentId && x.IsActive == true).GroupBy(x => x.IsActive).Select(x => new
                {
                    Target = x.Sum(c => c.AgentTarget) == null ? 0 : x.Sum(c => c.AgentTarget),
                }).Distinct().SingleOrDefault();
                if (Tget != null)
                {
                    ViewBag.Target = Convert.ToInt32(Tget.Target) - Convert.ToInt32(ViewBag.Inception);
                }
                else
                {
                    ViewBag.Target = 0;
                }
              
            }
            else {
             

                var Ince = objdb.Inceptions.Where(x=>x.AgentId==AgentId).GroupBy(x => x.AgentId).Select(x => new
                {
                    Inception = x.Sum(c => c.YTD_WAPI) == null ? 0 : x.Sum(c => c.YTD_WAPI),
                }).Distinct().SingleOrDefault();
                if (Ince != null)
                {
                    ViewBag.Inception = Ince.Inception;
                }
                else
                {
                    ViewBag.Inception = 0;
                }

                var Tget = objdb.TblUsers.Where(x => x.UserId == 3 && x.IsActive == true).GroupBy(x => x.UserId).Select(x => new
                {
                    Target = x.Sum(c => c.AgentTarget) == null ? 0 : x.Sum(c => c.AgentTarget),
                }).Distinct().SingleOrDefault();
                if (Tget != null)
                {
                    ViewBag.Target = Convert.ToInt32(Tget.Target) - Convert.ToInt32(ViewBag.Inception);
                }
                else
                {
                    ViewBag.Target = 0;
                }
            }


            return View(model);
        }
        #endregion Chart


        #region  AdminApproval
        public ActionResult AdminApproval()
        {
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            model.ManagerdistributeAmountList = new ManagerdistributeAmountBAL { }.ManagerdistributeAmountList();
            CustomMethods.ValidateRoles("Distribute Amount");
            return View(model);
        }

        public ActionResult AddEditAdminApprovalManagerdistributeListAmount(int IID = 0)
        {
            int UserId = Convert.ToInt32(Session["UserId"].ToString());
            ManagerdistributeAmountModel model = new ManagerdistributeAmountModel();
            if (UserId != 0)
            {
                ManagerdistributeAmountModel modela = new ManagerdistributeAmountBAL { }.GetManagerdistributeAmountById(IID);
                if (modela != null)
                {
                    model.ID = modela.ID;
                    model.AgentName = modela.AgentName;
                    model.AgentId = modela.AgentId;
                    model.ManagerId = modela.ManagerId;
                    model.Amount = modela.Amount;
                }
            }
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditAdminApprovalManagerdistributeListAmount(ManagerdistributeAmountModel model)
        {
            UserDocumentBAL catBLL = new UserDocumentBAL();
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Random rnd = new Random();
            try
            {
                long res = 0;
                long res1 = new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                {
                    ID = model.ID,
                    AgentId = model.AgentId,
                    ManagerId = model.ManagerId,
                    Amount = model.Amount,
                    Date = Date

                });
                if (res != 0)
                {
                    if (model.ID == 0)
                    {
                        Session["Success"] = "Amount added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Amount Updated successfully!";
                    }
                    return RedirectToAction("ManagerdistributeAmount");
                }
                return RedirectToAction("AdminApproval");
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                throw;
            }
        }
        public ActionResult ChangeManagerdistributeAmountsStatus(int id)
        {
            bool Result = false;
            bool ChangeStatus = new ManagerdistributeAmountBAL { }.ChangeManagerdistributeAmountsStatus(id);
            if (ChangeStatus)
            {
                Result = true;
            }
            TempData["msg"] = "<script>alert('Status Change Successfully');</script>";
            return RedirectToAction("DistributionList", "Home");
        }


        #endregion Chart

        #region  Inception
        public ActionResult Inception()
        {
            InceptionModel model = new InceptionModel();
            model.InceptionList = new InceptionBAL { }.AllInception();
            CustomMethods.ValidateRoles("Inception");
            return View(model);
        }
        public ActionResult AddEditInception(int id = 0)
        {
            InceptionModel model = new InceptionModel();
            if (id != 0)
            {
                InceptionModel obj = new InceptionBAL { }.GetInceptionById(id);
                if (obj != null)
                {
                    model.InceptionId = obj.InceptionId;
                    model.MonthFrom = obj.MonthFrom;

                    model.YearFrom = obj.YearFrom;

                    model.AgentId = obj.AgentId;
                    model.MTD_WAPI = obj.MTD_WAPI;
                    model.YTD_WAPI = obj.YTD_WAPI;
                }

            }
            model.AgentList = new UserBAL { }.AllUser().Where(x => x.RoleId == 3).ToList();
            //year
            List<SelectListItem> YearList = new List<SelectListItem>();
            for (int a = 2010; a <= 2030; a++)
            {
                YearList.Add(new SelectListItem { Text = a.ToString(), Value = a.ToString() });
            }
            //model.ListYear = YearList;

            //Month
            List<SelectListItem> MonthList = new List<SelectListItem>();
            MonthList.Add(new SelectListItem { Text = "Jan", Value = "1" });
            MonthList.Add(new SelectListItem { Text = "Feb", Value = "2" });
            MonthList.Add(new SelectListItem { Text = "Mar", Value = "3" });
            MonthList.Add(new SelectListItem { Text = "Apr", Value = "4" });
            MonthList.Add(new SelectListItem { Text = "May", Value = "5" });
            MonthList.Add(new SelectListItem { Text = "Jun", Value = "6" });
            MonthList.Add(new SelectListItem { Text = "Jul", Value = "7" });
            MonthList.Add(new SelectListItem { Text = "Aug", Value = "8" });
            MonthList.Add(new SelectListItem { Text = "Sep", Value = "9" });
            MonthList.Add(new SelectListItem { Text = "Oct", Value = "10" });
            MonthList.Add(new SelectListItem { Text = "Nov", Value = "11" });
            MonthList.Add(new SelectListItem { Text = "Dec", Value = "12" });
       
            //model.ListMonth = MonthList;
            model.GetType().GetProperty("ListYear").SetValue(model, YearList.Select(x => new SelectListItem { Value = x.Value, Text = x.Text }));
            model.GetType().GetProperty("ListMonth").SetValue(model, MonthList.Select(x => new SelectListItem { Value = x.Value, Text = x.Text }));

            CustomMethods.ValidateRoles("Inception");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddEditInception(InceptionModel model, FormCollection form)
        {
            InceptionBAL catBLL = new InceptionBAL();

            long UserId = Convert.ToInt32(Session["UserId"].ToString());

            string[] Agents = Request.Form.GetValues("Agent");
            string[] MTDAmount = Request.Form.GetValues("MTDAmount");
            string[] YTDAmount = Request.Form.GetValues("YTDAmount");
            
            try
            {
                var chk = new InceptionBAL { }.AllInception().Where(x => x.MonthFrom == model.MonthFrom && x.YearFrom == model.YearFrom).ToList();
                if (chk.Count == 0)
                {
                    long res = 0;
                    int i = 0;
                    foreach (var item in Agents)
                    {
                        model.AgentId = Convert.ToInt64(Agents[i].Trim());
                        model.MTD_WAPI = Convert.ToDecimal(MTDAmount[i].Trim());
                        model.YTD_WAPI = Convert.ToDecimal(YTDAmount[i].Trim());
                        res = new InceptionBAL { }.AddEditInception(new InceptionModel
                        {
                            InceptionId = model.InceptionId,
                            MonthFrom = model.MonthFrom,


                            YearFrom = model.YearFrom,

                            AgentId = model.AgentId,

                            MTD_WAPI = model.MTD_WAPI,
                            YTD_WAPI = model.YTD_WAPI,

                        });
                        i++;
                    }

                    if (res != 0)
                    {
                        if (model.InceptionId == 0)
                        {
                            Session["Success"] = "Inception added successfully!";
                        }
                        else
                        {
                            Session["Success"] = "Inception Updated successfully!";
                        }
                      
                    }
                    return RedirectToAction("UpdateInception");
                }
                else 
                {
                    Session["warning"] = "Sorry you have already added for this month!";
                    return RedirectToAction("AddeditInception");
                }

               
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }

        public ActionResult UpdateInception(int MF=0,int YF=0)
        {
            InceptionModel model = new InceptionModel();
             long UserId = Convert.ToInt32(Session["UserId"].ToString());
            Session["chk"] = "Update";
            if (MF > 0)
            {
                model.MonthFrom = MF;

                model.YearFrom = YF;
           
                model.InceptionList = new InceptionBAL { }.AllInception().Where(x => x.MonthFrom == MF  && x.YearFrom == YF).ToList();

                if (model.InceptionList.Count == 0)
                {
                    Session["chk"] = "New";
                    if (Session["RoleId"].ToString() == "4")
                    {
                        var agentList = new UserBAL { }.AllUser().Where(x => (x.RoleId == 3) && x.ManagerID == UserId).ToList();
                        model.InceptionList = agentList.Select(x => new InceptionModel
                        {
                            AgentId = x.UserId,
                            AgentName = x.Name,
                            Code=x.AgentCode,
                            RoleName=x.RoleName,
                            ManagerName = objdb.TblUsers.Where(y => y.UserId == x.UserId).Select(y => y.Name).SingleOrDefault(),
                            MTD_WAPI = 0,
                            YTD_WAPI = 0,
                        }).OrderByDescending(x => x.InceptionId).ToList();
                    }
                    else
                    {
                        var agentList = new UserBAL { }.AllUser().Where(x => x.RoleId == 3 || x.RoleId == 4).ToList();
                        model.InceptionList = agentList.Select(x => new InceptionModel
                        {
                            AgentId = x.UserId,
                            AgentName = x.Name,
                            Code = x.AgentCode,
                            RoleName = x.RoleName,
                            ManagerName = objdb.TblUsers.Where(y => y.UserId == x.UserId).Select(y => y.Name).SingleOrDefault(),
                            MTD_WAPI = 0,
                            YTD_WAPI = 0,
                        }).OrderByDescending(x => x.InceptionId).ToList();
                    }
                  

                }
            }
            else
            {
                model.InceptionList = new InceptionBAL { }.AllInception().Where(x => x.MonthFrom ==13).ToList();//just return list in null
            }
            //year
            List<SelectListItem> YearList = new List<SelectListItem>();
            for (int a = 2010; a <= 2030; a++)
            {
                YearList.Add(new SelectListItem { Text = a.ToString(), Value = a.ToString() });
            }
            //model.ListYear = YearList;

            //Month
            List<SelectListItem> MonthList = new List<SelectListItem>();
            MonthList.Add(new SelectListItem { Text = "Jan", Value = "1" });
            MonthList.Add(new SelectListItem { Text = "Feb", Value = "2" });
            MonthList.Add(new SelectListItem { Text = "Mar", Value = "3" });
            MonthList.Add(new SelectListItem { Text = "Apr", Value = "4" });
            MonthList.Add(new SelectListItem { Text = "May", Value = "5" });
            MonthList.Add(new SelectListItem { Text = "Jun", Value = "6" });
            MonthList.Add(new SelectListItem { Text = "Jul", Value = "7" });
            MonthList.Add(new SelectListItem { Text = "Aug", Value = "8" });
            MonthList.Add(new SelectListItem { Text = "Sep", Value = "9" });
            MonthList.Add(new SelectListItem { Text = "Oct", Value = "10" });
            MonthList.Add(new SelectListItem { Text = "Nov", Value = "11" });
            MonthList.Add(new SelectListItem { Text = "Dec", Value = "12" });

            //model.ListMonth = MonthList;
            model.GetType().GetProperty("ListYear").SetValue(model, YearList.Select(x => new SelectListItem { Value = x.Value, Text = x.Text }));
            model.GetType().GetProperty("ListMonth").SetValue(model, MonthList.Select(x => new SelectListItem { Value = x.Value, Text = x.Text }));

            CustomMethods.ValidateRoles("Inception");
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateInception(InceptionModel model, FormCollection form)
        {
            InceptionBAL catBLL = new InceptionBAL();

            long UserId = Convert.ToInt32(Session["UserId"].ToString());

            string[] InceptionId = Request.Form.GetValues("InceptId");
            string[] Agents = Request.Form.GetValues("Agent");
            string[] MTDAmount = Request.Form.GetValues("MTDAmount");
            string[] YTDAmount = Request.Form.GetValues("YTDAmount");

            try
            {
                long res = 0;
                int i = 0;
                foreach (var item in Agents)
                {
                    model.InceptionId = Convert.ToInt64(InceptionId[i].Trim());
                    model.AgentId = Convert.ToInt64(Agents[i].Trim());
                    model.MTD_WAPI = Convert.ToDecimal(MTDAmount[i].Trim());
                    model.YTD_WAPI = Convert.ToDecimal(YTDAmount[i].Trim());
                    res = new InceptionBAL { }.AddEditInception(new InceptionModel
                    {
                        InceptionId = model.InceptionId,
                        MonthFrom = model.MonthFrom,
              

                        YearFrom = model.YearFrom,
             
                        AgentId = model.AgentId,

                        MTD_WAPI = model.MTD_WAPI,
                        YTD_WAPI = model.YTD_WAPI,

                    });
                    i++;
                }

                if (res != 0)
                {
                    if (Session["chk"].ToString() == "New")
                    {
                        Session["Success"] = "Inception added successfully!";
                    }
                    else
                    {
                        Session["Success"] = "Inception Updated successfully!";
                    }
                    return RedirectToAction("UpdateInception", new { MF = model.MonthFrom, YF = model.YearFrom });
                }
                return View(model);
            }
            catch (Exception)
            {
                Session["Error"] = "An Error has occured";
                return View(model);
                throw;
            }
        }
        public ActionResult ExportInception(long yy = 0, long mm = 0)
        {

            InceptionModel model = new InceptionModel();
            //model.StudentRegistrationList = new StudentBAL { }.StudentListByBatch(id);
            var sttt = new UserBAL { }.AllUser().Where(x=>x.RoleId==3).ToList();

            DataTable tabGlobe = new DataTable();
            tabGlobe.Columns.Add(new DataColumn("AgentId", typeof(string)));
            tabGlobe.Columns.Add(new DataColumn("AgentName", typeof(string)));
            tabGlobe.Columns.Add(new DataColumn("MTD_WAPI", typeof(string)));
            tabGlobe.Columns.Add(new DataColumn("YTD_WAPI", typeof(string)));


            if (yy > 0)
            {
               
                var sss = new InceptionBAL { }.AllInception().Where(x => x.YearFrom==yy && x.MonthFrom==mm ).ToList();

                foreach (var item in sss)
                {
                    if (sss.Count > 0)
                    {
                        DataRow drnew = tabGlobe.NewRow();

                        drnew["AgentId"] = item.AgentId;
                        drnew["AgentName"] = item.AgentName;
                        drnew["MTD_WAPI"] = item.MTD_WAPI;
                        drnew["YTD_WAPI"] = item.YTD_WAPI;

                        tabGlobe.Rows.Add(drnew);
                    }
                }
            }
            else
            {
                foreach (var item in sttt)
                {
                    if (sttt.Count > 0)
                    {
                        DataRow drnew = tabGlobe.NewRow();

                        drnew["AgentId"] = item.UserId;
                        drnew["AgentName"] = item.Name;
                        drnew["MTD_WAPI"] = 0;
                        drnew["YTD_WAPI"] = 0;


                        tabGlobe.Rows.Add(drnew);
                    }
                }
            }

            DataView dv = new DataView(tabGlobe);
            if (tabGlobe.Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=AllInception.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                //Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                using (StringWriter sw = new StringWriter())
                {
                    HtmlTextWriter hw = new HtmlTextWriter(sw);
                    GridView gv = new GridView();
                    gv.DataSource = dv;
                    gv.DataBind();
                    gv.HeaderRow.BackColor = System.Drawing.Color.White;
                    foreach (TableCell cell in gv.HeaderRow.Cells)
                    {
                        cell.BackColor = gv.HeaderStyle.BackColor;
                    }
                    foreach (GridViewRow row in gv.Rows)
                    {
                        row.BackColor = System.Drawing.Color.White;
                        foreach (TableCell cell in row.Cells)
                        {
                            if (row.RowIndex % 2 == 0)
                            {
                                cell.BackColor = gv.AlternatingRowStyle.BackColor;
                            }
                            else
                            {
                                cell.BackColor = gv.RowStyle.BackColor;
                            }
                            cell.CssClass = "textmode";
                        }
                    }
                    gv.RenderControl(hw);

                    string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    //mail caode//
                    MemoryStream ms = new MemoryStream();

                    byte[] excelFile = System.Text.Encoding.ASCII.GetBytes(sw.ToString());
                    ms.Write(excelFile, 0, excelFile.Length);
                    ms.Position = 0;
                    Response.Flush();
                    Response.End();
                }

            }

            return RedirectToAction("UpdateInception");
        }
        [HttpPost]
        public ActionResult ImportInception(InceptionModel model, HttpPostedFileBase file)
        {
            InceptionBAL BLL = new InceptionBAL();
            //try
            //{

            Random rnd = new Random();
            DataSet ds = new DataSet();

            long res = 0;
            //var chk = new InceptionBAL { }.AllInception().Where(x => x.MonthFrom == model.MonthFrom && x.YearFrom == model.YearFrom).ToList();
            //if (chk.Count() == 0)
            //{
                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Server.MapPath("~/Images/AgentExcel/") + "Std" + rnd.Next(100, 100000) + Request.Files["file"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        Request.Files["file"].SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        System.Data.OleDb.OleDbConnection excelConnection = new System.Data.OleDb.OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        System.Data.OleDb.OleDbConnection excelConnection1 = new System.Data.OleDb.OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (System.Data.OleDb.OleDbDataAdapter dataAdapter = new System.Data.OleDb.OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                    }
                    if (fileExtension.ToString().ToLower().Equals(".xml"))
                    {
                        string fileLocation = Server.MapPath("~/ExcelFile/") + Request.Files["FileUpload"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        Request.Files["FileUpload"].SaveAs(fileLocation);
                        System.Xml.XmlTextReader xmlreader = new System.Xml.XmlTextReader(fileLocation);
                        // DataSet ds = new DataSet();
                        ds.ReadXml(xmlreader);
                        xmlreader.Close();
                    }



                    ///insertion of Excel in Database
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long Id = 0;
                        model.AgentId = Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString());
                        var chkExixt = objdb.Inceptions.Where(x => x.AgentId == model.AgentId && x.YearFrom == model.YearFrom && x.MonthFrom == model.MonthFrom).SingleOrDefault();
                        if (chkExixt != null)
                        {
                            Id = chkExixt.InceptionId;
                        }

                        model.MTD_WAPI = Convert.ToDecimal(ds.Tables[0].Rows[i][2].ToString());
                        model.YTD_WAPI = Convert.ToDecimal(ds.Tables[0].Rows[i][3].ToString());



                                     res = new InceptionBAL { }.AddEditInception(new InceptionModel
                                    {
                                        InceptionId = Id,
                                        AgentId = model.AgentId,
                                        MTD_WAPI = model.MTD_WAPI,
                                        YTD_WAPI = model.YTD_WAPI,
                                        MonthFrom = model.MonthFrom,
                                        YearFrom = model.YearFrom,

                                    });

                    }

                }
                Session["success"] = "Imported Successfully!";
                return RedirectToAction("UpdateInception");
            //}

            //else
            //{
            //    Session["warning"] = "Sorry you have already added for this month!";
            //    return RedirectToAction("UpdateInception");
            //}





        }

        #endregion Inception

        #region AgentTarget
        public ActionResult AgentTarget()
        {
            UserModel model = new UserModel();
            model.UserList = new UserBAL { }.AllUser().Where(x=>x.RoleId==3  || x.RoleId==4).ToList();
            CustomMethods.ValidateRoles("User");
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AgentTarget(FormCollection form)
        {

            string[] AgentId = Request.Form.GetValues("AgentId");
            string[] AgentTarget = Request.Form.GetValues("AgentTarget");

                long res = 0;
                int i = 0;
                foreach (var item in AgentId)
                {
                    long AgtId = Convert.ToInt64(AgentId[i].Trim());
                    decimal AgntTarget = Convert.ToDecimal(AgentTarget[i].Trim());


                    var objUser = objdb.TblUsers.Find(AgtId);
                    objUser.AgentTarget = AgntTarget;
                    objdb.SaveChanges();

                    i++;
                }


                Session["Success"] = "Agent Target has been set successfully!";
            return RedirectToAction("AgentTarget");
        }

        public ActionResult ExportAgent()
        {

            InceptionModel model = new InceptionModel();
            //model.StudentRegistrationList = new StudentBAL { }.StudentListByBatch(id);
            var sttt = new UserBAL { }.AllUser().Where(x => x.RoleId == 3).ToList();

            DataTable tabGlobe = new DataTable();
            tabGlobe.Columns.Add(new DataColumn("AgentId", typeof(string)));
            tabGlobe.Columns.Add(new DataColumn("AgentName", typeof(string)));
            tabGlobe.Columns.Add(new DataColumn("Target", typeof(string)));
            tabGlobe.Columns.Add(new DataColumn("Manager", typeof(string)));



            foreach (var item in sttt)
            {
                if (sttt.Count > 0)
                {
                    DataRow drnew = tabGlobe.NewRow();

                    drnew["AgentId"] = item.UserId;
                    drnew["AgentName"] = item.Name;
                    drnew["Target"] = item.AgentTarget;
                    drnew["Manager"] = item.ManagerName;


                    tabGlobe.Rows.Add(drnew);
                }
            }
            DataView dv = new DataView(tabGlobe);
            if (tabGlobe.Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=AgentTarget.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                using (StringWriter sw = new StringWriter())
                {
                    HtmlTextWriter hw = new HtmlTextWriter(sw);
                    GridView gv = new GridView();
                    gv.DataSource = dv;
                    gv.DataBind();
                    gv.HeaderRow.BackColor = System.Drawing.Color.White;
                    foreach (TableCell cell in gv.HeaderRow.Cells)
                    {
                        cell.BackColor = gv.HeaderStyle.BackColor;
                    }
                    foreach (GridViewRow row in gv.Rows)
                    {
                        row.BackColor = System.Drawing.Color.White;
                        foreach (TableCell cell in row.Cells)
                        {
                            if (row.RowIndex % 2 == 0)
                            {
                                cell.BackColor = gv.AlternatingRowStyle.BackColor;
                            }
                            else
                            {
                                cell.BackColor = gv.RowStyle.BackColor;
                            }
                            cell.CssClass = "textmode";
                        }
                    }
                    gv.RenderControl(hw);

                    string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    //mail caode//
                    MemoryStream ms = new MemoryStream();

                    byte[] excelFile = System.Text.Encoding.ASCII.GetBytes(sw.ToString());
                    ms.Write(excelFile, 0, excelFile.Length);
                    ms.Position = 0;
                    Response.Flush();
                    Response.End();
                }

            }

            return RedirectToAction("AgentTarget");
        }

        [HttpPost]
        public ActionResult ImportAgent(HttpPostedFileBase file)
        {

            //try
            //{
            UserModel model =new UserModel();
            Random rnd = new Random();
            DataSet ds = new DataSet();

            long res = 0;

                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Server.MapPath("~/Images/AgentExcel/") + "AgentTarget" + rnd.Next(100, 100000) + Request.Files["file"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        Request.Files["file"].SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        System.Data.OleDb.OleDbConnection excelConnection = new System.Data.OleDb.OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        System.Data.OleDb.OleDbConnection excelConnection1 = new System.Data.OleDb.OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (System.Data.OleDb.OleDbDataAdapter dataAdapter = new System.Data.OleDb.OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                    }
                    if (fileExtension.ToString().ToLower().Equals(".xml"))
                    {
                        string fileLocation = Server.MapPath("~/ExcelFile/") + Request.Files["FileUpload"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        Request.Files["FileUpload"].SaveAs(fileLocation);
                        System.Xml.XmlTextReader xmlreader = new System.Xml.XmlTextReader(fileLocation);
                        // DataSet ds = new DataSet();
                        ds.ReadXml(xmlreader);
                        xmlreader.Close();
                    }



                    ///insertion of Excel in Database
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        model.UserId = Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString());
                        model.AgentTarget = Convert.ToDecimal(ds.Tables[0].Rows[i][2].ToString());

                        var objUser = objdb.TblUsers.Find(model.UserId);
                        objUser.AgentTarget = model.AgentTarget;
                        objdb.SaveChanges();

                    }

                }

          


            Session["success"] = "Imported Successfully!";
            return RedirectToAction("AgentTarget");




        }
        #endregion

        public int SendNotification(int UserId, string title, string message)
        {

            var ret = objdb.TblUsers.Where(x => x.DeviceId != null && x.UserId == UserId).SingleOrDefault();
            try
            {
                //Legacy server key
                //var applicationID = "AIzaSyAAmq-LgPCTrr82JnEKn_CSqh5poeyNH-I";
                var applicationID = "AIzaSyDOAYuOeNFgSMBxfi5DqummMi9l4gpp10k";
                var senderId = "585507246258";
                if (ret != null)
                {
                    string deviceId = ret.DeviceId;
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";

                    //string ss = "noti";
                    var datass = new
                    {
                        to = deviceId,
                        data = new
                        {
                            body = message,
                            title = title,
                            icon = "myicon"
                        }
                    };

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(datass);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    Response.Write(sResponseFromServer);
                                }
                            }
                        }



                    }
                }

            }

            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

            return 1;
        }

    }
}

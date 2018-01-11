using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using JcoDAL;
using CommonUtility;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Drawing;
using JcoBAL;
using System.Xml.Linq;
using Jco.Filters;
using System.Web;
using System.Windows.Forms;
using System.Web.Providers.Entities;

namespace Jco.Controllers
{
   [RoutePrefix("api/JCOService")]
    public class JCOServiceController : ApiController
    {

        string adminEmail = System.Configuration.ConfigurationSettings.AppSettings["AdminEmail"];
        string MailEmail = System.Configuration.ConfigurationSettings.AppSettings["MailUid"];
        string MailPassword = System.Configuration.ConfigurationSettings.AppSettings["MailPwd"];
        string host = System.Configuration.ConfigurationSettings.AppSettings["smtpAddress"];

        JCOEntities objbd = new JCOEntities();

        //string ImageURL = "http://localhost:3853/Images/Users/";
        //string UserDocumentsURL = "http://localhost:3853/Images/UserDocuments/";
        //string UserManagerDocumentsURL = "http://localhost:3853/Images/ManagerDocument/";
        //string AnnouncementsURL = "http://localhost:3853/Images/Announcements/";
        //string PollingURL = "http://localhost:3853/Images/Polling/";

        string ImageURL = "http://jc.kazmatechnology.in/Images/Users/";
        string UserDocumentsURL = "http://jc.kazmatechnology.in/Images/UserDocuments/";
        string AnnouncementsURL = "http://jc.kazmatechnology.in/Images/Announcements/";
        string PollingURL = "http://jc.kazmatechnology.in/Images/Polling/";

        TimeZoneInfo IND_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        [Route("AboutJCO")]
        [HttpPost]
        public HttpResponseMessage AboutJCO()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            
            try
            {
                var adminDet = objbd.CompanyProfiles.Select(x => x.Description).FirstOrDefault();
                if (adminDet!=null)
                {
                    Value["result"] = "TRUE";
                    Value["AboutJCO"] = System.Text.RegularExpressions.Regex.Replace(adminDet, @"<[^>]+>|&nbsp;", string.Empty);
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("LoginUser")]
        [HttpPost]
        public HttpResponseMessage LoginUser()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            string Email = System.Web.HttpContext.Current.Request.Form["Email"];
            string password = System.Web.HttpContext.Current.Request.Form["password"];
            string DeviceId = System.Web.HttpContext.Current.Request.Form["DeviceId"];
            password = DataEncryption.Encrypt(password, "passKey");
            try
            {
                var chkDup = objbd.TblUsers.Where(x => x.EmailId == Email & x.Password == password).SingleOrDefault();
                if (chkDup.DeviceId != DeviceId)
                {
                    var obj = objbd.TblUsers.Find(chkDup.UserId);
                    obj.DeviceId = DeviceId;
                    objbd.SaveChanges();
                }
                var sd = objbd.TblUsers.Where(x => x.EmailId == Email & x.Password == password).ToList();
              
                var adminDet = sd.Select(x => new
                {
                    UserId = x.UserId,
                    RollId = x.RoleId,
                    Name = x.Name,
                    Gender = x.Gender,
                    DOB = x.DOB,
                    Email = x.EmailId,
                    Mobile = x.Mobile,
                    Photo = ImageURL + x.Photo,
                    AgentCode = x.AgentCode,
                    Designation = x.Role.RoleName,
                    DateOfJoined = x.JoinDate,
                    //DeviceId = x.DeviceId,
                    AwardList = objbd.Awards.Where(y => y.UserId == x.UserId).Select(y => new 
                    {
                       AwardTitle=y.Title,
                    }).ToList(),
                }).ToList();

                if (adminDet.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["LoginValue"] = adminDet.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public HttpResponseMessage ForgotPassword()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            string Email = System.Web.HttpContext.Current.Request.Form["Email"];
            try
            {
                var adminDet = objbd.TblUsers.Where(x => x.EmailId == Email).Select(x => x.Password).Single();
                if(adminDet!=null)
                {
                    string password = DataEncryption.Decrypt(adminDet.Trim(), "passKey");

                    string txt = "You have forgot your password on JCO. Your Password is : " + password + "";
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(adminEmail);
                    message.To.Add(new MailAddress(Email));
                    message.Subject = "Forgot password on JCO";
                    message.Body = "message: " + txt + "\r\n" + adminEmail;
                    SmtpClient smtp = new SmtpClient(host, 25);
                    message.Priority = MailPriority.Normal;
                    smtp.Credentials = new System.Net.NetworkCredential(MailEmail, MailPassword);
                    smtp.Timeout = 60000;
                    smtp.EnableSsl = false;
                    smtp.Send(message);
                }
                if (adminDet!=null)
                {
                    Value["result"] = "TRUE";                 
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("Profile")]
        [HttpPost]
        public HttpResponseMessage Profile()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var sd=objbd.TblUsers.Where(x => x.UserId == UserId).ToList();
                var adminDet = sd.Select(x => new
                {
                    UserId = x.UserId,
                    RollId = x.RoleId,
                    Name = x.Name,
                    Gender = x.Gender,
                    DOB = x.DOB,
                    Email = x.EmailId,
                    Mobile = x.Mobile,
                    Photo = ImageURL + x.Photo,
                    AgentCode = x.AgentCode,
                    Designation = x.Role.RoleName,
                    DateOfJoined = x.JoinDate,
                    AwardList = objbd.Awards.Where(y => y.UserId == x.UserId).Select(y => new
                    {
                        AwardTitle = y.Title,
                    }).ToList(),

                }).ToList();

                if (adminDet.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["LoginValue"] = adminDet.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("AllAgent")]
        [HttpPost]
        public HttpResponseMessage AllAgent()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            try
            {
                var sd = objbd.TblUsers.ToList();
                var adminDet = sd.Select(x => new
                {
                    UserId = x.UserId,
                    RollId = x.RoleId,
                    Name = x.Name,
                    Gender = x.Gender,
                    DOB = x.DOB,
                    Email = x.EmailId,
                    Mobile = x.Mobile,
                    Photo = ImageURL + x.Photo,
                    AgentCode = x.AgentCode,
                    Designation = x.Role.RoleName,
                    DateOfJoined = x.JoinDate,

                }).ToList();

                if (adminDet.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["AgentList"] = adminDet.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        public class Document
        {
            public long UserDocumentId { get; set; }
            public string Uploader { get; set; }
            public string FileName { get; set; }
            public string DocumentName { get; set; }
            public string Ext { get; set; }
            public string FileType { get; set; }
            public string Size { get; set; }
        }

        [Route("GetUserDocument")]
        [HttpPost]
        public HttpResponseMessage GetUserDocument()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            var rol = objbd.TblUsers.Where(x => x.UserId == UserId).SingleOrDefault();
            var chkPermission = objbd.RoleAssignments.Where(x => x.RoleId == rol.RoleId && x.ModuleId == 12).SingleOrDefault();
            try
            {
                //var list = objbd.UserDocuments.Where(x => x.UserId == UserId).Select(x => new {
                var list = objbd.UserDocuments.Select(x => new
                {
                    Id = x.UserDocumentId,
                    DocType = x.DocType,
                    Document = x.DocumentName,
                    Ext = x.DocumentExtension,
                    UserId = x.UserId,
                    Uploader=x.TblUser.Name,
                }).ToList();
                List<Document> Document = new List<Document>();
                List<Document> ManagerDocumentl = new List<Document>();
                long UserID = 0;
                foreach(var item in list)
                {
                    UserID = item.UserId;
                    long Id = item.Id;
                    string Ddocument = UserDocumentsURL + item.Document;
                    string ManagerDocument = UserDocumentsURL + item.Document;
                    string Ext = item.Ext; 
                    string FileType = item.Ext.Replace(".", "");
                    string document = "";
                    if (item.DocType=="User")
                    {
                        if (item.Document != "" && item.Document != null)
                        {
                            WebClient webClient = new WebClient();
                            webClient.OpenRead(Ddocument);
                            long totalSizeBytes = Convert.ToInt64(webClient.ResponseHeaders["Content-Length"]);
                            float KbValue = (totalSizeBytes / 1024);
                            document = Ddocument;
                            Document.Add(new Document { UserDocumentId = Id, Uploader =item.Uploader , FileName = item.Document, DocumentName = document, Ext = Ext, FileType = FileType, Size = KbValue.ToString() + " " + "Kb" });
                        }

                    }
                    else
                    {
                        if (chkPermission.ViewRecord == true)
                        {
                            if (item.Document != "" && item.Document != null)
                            {
                                WebClient webClient = new WebClient();
                                webClient.OpenRead(ManagerDocument);
                                long totalSizeBytes = Convert.ToInt64(webClient.ResponseHeaders["Content-Length"]);
                                float KbValue = (totalSizeBytes / 1024);
                                document = ManagerDocument;
                                ManagerDocumentl.Add(new Document { UserDocumentId = Id, Uploader = item.Uploader, FileName = item.Document, DocumentName = document, Ext = Ext, FileType = FileType, Size = KbValue.ToString() + " " + "Kb" });
                            }
                        }
                        else
                        {
                            Value["result"] = "FALSE";
                            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
                            return response;
                        }

                    }
                   
                    
                }
                if (list.Count>0)
                {
                    Value["result"] = "TRUE";
                    Value["UserId"] = UserID.ToString();
                    if (rol.RoleId == 3)
                    {
                        Value["General"] = Document.ToArray();
                     
                    }
                    else 
                    {
                        Value["General"] = Document.ToArray();
                        Value["Managenent"] = ManagerDocumentl.ToArray();
                    }
                    
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("DeleteDocument")]
        [HttpPost]
        public HttpResponseMessage DeleteDocument()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DocumentId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DocumentId"]);

            try
            {
                var chkDoc = objbd.UserDocuments.Where(x => x.UserDocumentId == DocumentId).SingleOrDefault();
                if (chkDoc.UserId == UserId)
                {
                    var rol = objbd.TblUsers.Where(x => x.UserId == UserId).SingleOrDefault();
                    var chkPermission = objbd.RoleAssignments.Where(x => x.RoleId == rol.RoleId && x.ModuleId == 12).SingleOrDefault();
                    if (chkPermission.DeleteRecord != true)
                    {
                        Value["result"] = "FALSE";
                        response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
                        return response;
                    }
                    var pri1 = (from ok in objbd.UserDocuments where DocumentId == ok.UserDocumentId select ok).Single();
                    objbd.UserDocuments.Attach(pri1);
                    objbd.UserDocuments.Remove(pri1);
                    objbd.SaveChanges();
                    if (pri1 != null)
                    {
                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
                }
                else 
                {
                    Value["result"] = "FALSE";
                }


            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("UserDocumentUpload")]
        [HttpPost]
        public HttpResponseMessage UserDocumentUpload()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            var File = System.Web.HttpContext.Current.Request.Files["File"];
            var FileType = System.Web.HttpContext.Current.Request.Form["FileType"];
            string extn = "";
            Random rnd = new Random();
            //string FileType = "";

            string strFileName = "";
            string path = "";
            try
            {
                var chkUser = objbd.TblUsers.Where(x => x.UserId == UserId).SingleOrDefault();
                if (FileType == "User")
                {
                    FileType = "User";
                }
                else
                {
                    FileType = "Manager";
                }

                if (File != null)
                {
                    strFileName = "UserDocumentImg_" + rnd.Next(100, 100000000) + "." + File.FileName.Split('.')[1].ToString();
                    path = System.Web.HttpContext.Current.Server.MapPath("~/Images/UserDocuments/" + strFileName);
                    extn = System.IO.Path.GetExtension(path);
                    File.SaveAs(path);
                }
                var res = new UserDocumentBAL { }.AddEditUserDocument(new UserDocumentModel
                {
                    UserDocumentId = 0,
                    DocumentName = strFileName,
                    DocumentExtension = extn,
                    UserId = UserId,
                    DocType=FileType,

                });
                if (res != null)
                {
                    Value["result"] = "TRUE";
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("NewEvent")]
        [HttpPost]
        public HttpResponseMessage NewEvent()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);

            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            try
            {
                var lst = objbd.Events.ToList();
                var list1 = lst.Select(x => new
                {
                    EventId = x.EventId,
                    Date = x.EventDate,
                    EventName = x.EventName,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = x.RSVP_By,
                    RSVP_Time = x.RSVP_Time,
                    IsGuestRegistration = x.IsGuestRegistration,
                    AgentRsvp=objbd.AgentEventParticipates.Where(y=>y.EventId==x.EventId && y.UserId==UserId).ToList().Count(),
                }).OrderByDescending(x => x.EventId).ToList();
                var list = list1.Where(x => x.AgentRsvp==0 && Date.Date<x.Date.Date).Select(x => new
                {
                    EventId = x.EventId,
                    Date = Convert.ToDateTime(x.Date).ToShortDateString(),
                    EventName = x.EventName,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = Convert.ToDateTime(x.RSVP_By).ToShortDateString(),
                    RSVP_Time = x.RSVP_Time,
                    IsGuestRegistration = x.IsGuestRegistration,
                }).OrderByDescending(x => x.EventId).ToList();
                if (list.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["Event"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("ActiveEvent")]
        [HttpPost]
        public HttpResponseMessage ActiveEvent()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);

            try
            {
                var lst = objbd.Events.ToList();
                var list1 = lst.Select(x => new
                {
                    EventId = x.EventId,
                    Date = x.EventDate,
                    EventName = x.EventName,
                    Desctiption = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = x.RSVP_By,
                    RSVP_Time = x.RSVP_Time,
                    IsGuestRegistration = x.IsGuestRegistration,
                    AgentRsvp = objbd.AgentEventParticipates.Where(y => y.EventId == x.EventId && y.UserId == UserId).ToList().Count(),
                }).OrderByDescending(x => x.EventId).ToList();
                var list = list1.Where(x => x.AgentRsvp > 0 && x.Date.Date >= Date.Date).Select(x => new
                {
                    EventId = x.EventId,
                    Date = Convert.ToDateTime(x.Date).ToShortDateString(),
                    EventName = x.EventName,
                    //Desctiption = x.Desctiption,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = Convert.ToDateTime(x.RSVP_By).ToShortDateString(),
                    RSVP_Time = x.RSVP_Time,
                    IsGuestRegistration = x.IsGuestRegistration,
                }).OrderByDescending(x => x.EventId).ToList();
                if (list.Count>0)
                {
                    Value["result"] = "TRUE";
                    Value["ActiveEvent"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("PastEvent")]
        [HttpPost]
        public HttpResponseMessage PastEvent()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);

            try
            {
             
                var list1 = objbd.Events.Select(x => new { 
                    EventId = x.EventId,
                    Date = x.EventDate,
                    EventName = x.EventName,
                    Desctiption = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = x.RSVP_By,
                    RSVP_Time = x.RSVP_Time,
                    IsGuestRegistration = x.IsGuestRegistration,
                }).OrderByDescending(x => x.EventId).ToList();
                var list = list1.Where(x => x.Date.Date <= Date.Date).Select(x => new
                {
                    EventId = x.EventId,
                    Date = Convert.ToDateTime(x.Date).ToShortDateString(),
                    EventName = x.EventName,
                    //Desctiption = x.Desctiption,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = Convert.ToDateTime(x.RSVP_By).ToShortDateString(),
                    RSVP_Time = x.RSVP_Time,
                    IsGuestRegistration = x.IsGuestRegistration,
                }).OrderByDescending(x => x.EventId).ToList();
                if (list.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["PastEvent"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("Agentparticipate")]
        [HttpPost]
        public HttpResponseMessage Agentpaticipate()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long EventId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["EventId"]);

            try
            {
                var list = objbd.AgentEventParticipates.Where(x=>x.UserId==UserId && x.EventId==EventId).ToList();
                if (list.Count == 0)
                {
                    AgentEventParticipate obj = new AgentEventParticipate
                    {
                        EventId = EventId,
                        UserId = UserId,
                        Date=Date,
                    };
                    objbd.AgentEventParticipates.Add(obj);
                    objbd.SaveChanges();
                    long res = obj.EventparticipateId;
                    if (res != null)
                    {
                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }                   
                }
                else
                {
                    Value["result"] = "FALSE";
                    Value["Message"] = "AlreAdy Added";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }
        [Route("DeleteEvent")]
        [HttpPost]
        public HttpResponseMessage DeleteEvent()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long EventId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["EventId"]);

            try
            {
                long RollId = objbd.TblUsers.Where(x => x.UserId == UserId).Select(x => x.RoleId).SingleOrDefault();
                var roll = objbd.RoleAssignments.Where(x => x.ModuleId == 4 & x.RoleId == RollId).Select(x => x.DeleteRecord).SingleOrDefault();
                if (Convert.ToBoolean(roll) == true)
                {
                    var list = objbd.AgentEventParticipates.Where(x => x.EventId == EventId).ToList();
                    if (list.Count != 0)
                    {
                        foreach (var item in list)
                        {
                            var pri1 = (from ok in objbd.AgentEventParticipates where item.EventparticipateId == ok.EventparticipateId select ok).Single();
                            objbd.AgentEventParticipates.Attach(pri1);
                            objbd.AgentEventParticipates.Remove(pri1);
                            objbd.SaveChanges();
                        }
                    }
                    var list1 = objbd.Guests.Where(x => x.EventId == EventId).ToList();
                    if (list1.Count != 0)
                    {
                        foreach (var item in list1)
                        {
                            var pri1 = (from ok in objbd.Guests where item.GuestId == ok.GuestId select ok).Single();
                            objbd.Guests.Attach(pri1);
                            objbd.Guests.Remove(pri1);
                            objbd.SaveChanges();
                        }
                    }

                    var pri = (from ok in objbd.Events where EventId == ok.EventId select ok).Single();
                    objbd.Events.Attach(pri);
                    objbd.Events.Remove(pri);
                    objbd.SaveChanges();
                    if (pri != null)
                    {
                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
                }
                else
                {
                    Value["result"] = "FALSE";
                }                
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("Guestparticipate")]
        [HttpPost]
        public HttpResponseMessage Guestpaticipate()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long EventId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["EventId"]);
            string GustName = System.Web.HttpContext.Current.Request.Form["GustName"];
            string NRIC_OR_ID = System.Web.HttpContext.Current.Request.Form["NRIC_OR_ID"];
            string Age = System.Web.HttpContext.Current.Request.Form["Age"];
            string GuestEmail = System.Web.HttpContext.Current.Request.Form["GuestEmail"];
            string Mobile = System.Web.HttpContext.Current.Request.Form["Mobile"];

            try
            {

                var list = objbd.AgentEventParticipates.Where(x => x.UserId == UserId && x.EventId == EventId).ToList();
                if (list.Count == 0)
                {
                    AgentEventParticipate objS = new AgentEventParticipate
                    {
                        EventId = EventId,
                        UserId = UserId,
                        Date = Date,
                    };
                    objbd.AgentEventParticipates.Add(objS);
                    objbd.SaveChanges();
                   
                  
                }

                    Guest obj = new Guest
                    {
                        EventId = EventId,
                        AgentId = UserId,
                        GuestName = GustName,
                        NRIC_OR_ID = NRIC_OR_ID,
                        Age = Age,
                        GuestEmail = GuestEmail,
                        Mobile = Mobile,
                        Date=Date,
                    };
                    objbd.Guests.Add(obj);
                    objbd.SaveChanges();
                    long res = obj.GuestId;
                    if (res != null)
                    {
                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("EventDetails")]
        [HttpPost]
        public HttpResponseMessage EventDetails()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long EventId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["EventId"]);
            try
            {
                //var list1 = objbd.AgentEventParticipates.Where(x=>x.EventId==EventId && x.UserId==UserId).Select(x => new
                //{
                //    EventId = x.EventId,
                //    Date = x.Event.EventDate,
                //    EventName = x.Event.EventName,
                //    Desctiption = x.Event.Description,
                //    StartTime = x.Event.StartTime,
                //    EndTime = x.Event.EndTime,
                //    RSVP_By = x.Event.RSVP_By,
                //    RSVP_Time = x.Event.RSVP_Time,
                //    AgentTotalSit = x.Event.AgentSeat,
                //    TotalAgentSitBooked = objbd.AgentEventParticipates.Where(y => y.EventId == x.EventId).Count(),
                //    GaustTotalSit = x.Event.GuestSeat,
                //    TotalGustSitBooked = objbd.Guests.Where(z => z.EventId == x.EventId).Count()
                //}).ToList();
                var list1 = objbd.Events.Where(x => x.EventId == EventId).Select(x => new
                {
                    EventId = x.EventId,
                    Date = x.EventDate,
                    EventName = x.EventName,
                    Desctiption = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = x.RSVP_By,
                    RSVP_Time = x.RSVP_Time,
                    AgentTotalSit = x.AgentSeat,
                    TotalAgentSitBooked = objbd.AgentEventParticipates.Where(y => y.EventId == x.EventId).Count(),
                    GaustTotalSit = x.GuestSeat,
                    TotalGustSitBooked = objbd.Guests.Where(z => z.EventId == x.EventId).Count()
                }).ToList();

                var list = list1.Select(x => new
                {
                    EventId = x.EventId,
                    Date = Convert.ToDateTime(x.Date).ToShortDateString(),
                    EventName = x.EventName,
                    Desctiption = x.Desctiption,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    RSVP_By = Convert.ToDateTime(x.RSVP_By).ToShortDateString(),
                    RSVP_Time = x.RSVP_Time,
                    AgentTotalSit = x.AgentTotalSit,
                    TotalAgentSitBooked = x.TotalAgentSitBooked,
                    GaustTotalSit = x.GaustTotalSit,
                    TotalGustSitBooked = x.TotalGustSitBooked,
                    GuestList = objbd.Guests.Where(y => y.EventId == x.EventId).Select(y => new {
                        GuestId = y.GuestId,
                        GuestName=y.GuestName,
                        Mobile=y.Mobile,
                        NRIC_OR_ID=y.NRIC_OR_ID,
                        Age=y.Age,
                        Email=y.GuestEmail,
                    
                    }),
                }).ToList();
                if (list.Count>0)
                {
                    Value["result"] = "TRUE";
                    Value["EventDetails"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("CancelRVPS")]
        [HttpPost]
        public HttpResponseMessage CancelRVPS()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long EventId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["EventId"]);

            //var rol = objbd.TblUsers.Where(x => x.UserId == UserId).SingleOrDefault();
            //var chkPermission = objbd.RoleAssignments.Where(x => x.RoleId == rol.RoleId && x.ModuleId == 4).SingleOrDefault();
            //if (chkPermission.DeleteRecord != true)
            //{
            //    Value["result"] = "FALSE";
            //    response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            //    return response;
            //}
            try
            {
                long AgentperticiptId = objbd.AgentEventParticipates.Where(x => x.UserId == UserId & x.EventId== EventId).Select(x => x.EventparticipateId).SingleOrDefault();
                if (AgentperticiptId != 0)
                {

                    //RemoveGuest

                    objbd.Guests.Where(p => p.EventId == EventId)
                       .ToList().ForEach(p => objbd.Guests.Remove(p));
                    objbd.SaveChanges();



                    var pri1 = (from ok in objbd.AgentEventParticipates where AgentperticiptId == ok.EventparticipateId select ok).Single();
                    objbd.AgentEventParticipates.Attach(pri1);
                    objbd.AgentEventParticipates.Remove(pri1);
                    objbd.SaveChanges();
                    if (pri1!=null)
                    {
                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("CancelRVPSGuest")]
        [HttpPost]
        public HttpResponseMessage CancelRVPSGuest()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
             DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long EventId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["EventId"]);
            long GuestId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["GuestId"]);

            var rol = objbd.TblUsers.Where(x => x.UserId == UserId).SingleOrDefault();
            var chkPermission = objbd.RoleAssignments.Where(x => x.RoleId == rol.RoleId && x.ModuleId == 4).SingleOrDefault();
            if (chkPermission.DeleteRecord != true)
            {
                Value["result"] = "FALSE";
                response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
                return response;
            }
            try
            {
                var chkEvent = objbd.Events.Where(x => x.EventId == EventId && x.EventDate.Date > Date.Date).SingleOrDefault();
                if (chkEvent != null)
                {
                    //RemoveGuest

                    objbd.Guests.Where(p => p.GuestId == GuestId)
                       .ToList().ForEach(p => objbd.Guests.Remove(p));
                    objbd.SaveChanges();

                    Value["result"] = "TRUE";
                }
                else {
                    Value["result"] = "FALSE";
                }

            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }


        [Route("PollList")]
        [HttpPost]
        public HttpResponseMessage PollList()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            HttpResponseMessage response = null;
            try
            {
                var list1 = objbd.AssignPollingAgents.Where(x=>x.AgentId==UserId).Select(x => new
                {
                    PollId = x.PollId,
                    Title = x.Poll.Title,
                    Image = x.Poll.Image == "" ? null : PollingURL + x.Poll.Image,
                    Description = x.Poll.Description,
                    CreatedDate = x.Poll.CreatedDate,
                    CreatedByUserId = x.Poll.CreatedBy,
                    CreatedByUserName = objbd.TblUsers.Where(y => y.UserId == x.Poll.CreatedBy).Select(y => y.Name).FirstOrDefault(),
                    VoteLastDate = x.Poll.VoteLastDate,
                    IsActive = x.Poll.IsActive,
                    Choice = objbd.PollOptions.Where(y => y.PollId == x.PollId).Select(y => new
                    {
                        //PollId = x.PollId,
                        PollOptionId = y.PollOptionId,
                        Options = y.Options,
                        //TotalPoll = objbd.PollResults.Where(p => p.PollId == y.PollId).ToList().Count(),
                        //OptionPoll = objbd.PollResults.Where(p => p.ResultId == y.PollOptionId).ToList().Count(),
                    })
                }).ToList();
                var list = list1.Select(x => new
                {
                    PollId = x.PollId,
                    Title = x.Title,
                    Image = x.Image,
                    Description = x.Description,
                    CreatedDate = Convert.ToDateTime(x.CreatedDate).ToShortDateString(),
                    CreatedByUserId = x.CreatedByUserId,
                    CreatedByUserName = x.CreatedByUserName,
                    VoteLastDate = Convert.ToDateTime(x.VoteLastDate).ToShortDateString(),
                    IsActive = x.IsActive,
                    Choice = x.Choice,
                }).ToList();

                //var lists = list.Select(x => new,
                //{
                //    PollId = x.PollId,
                //    Title = x.Title,
                //    Image = x.Image,
                //    Description = x.Description,
                //    CreatedDate = Convert.ToDateTime(x.CreatedDate).ToShortDateString(),
                //    CreatedByUserId = x.CreatedByUserId,
                //    CreatedByUserName = x.CreatedByUserName,
                //    VoteLastDate = Convert.ToDateTime(x.VoteLastDate).ToShortDateString(),
                //    IsActive = x.IsActive,
                //    Choice = x.Choice.Select(y => new
                //    {
                //        PollOptionId = y.PollOptionId,
                //        Options = y.Options,
                //        //TotalPoll = objbd.PollResults.Where(p => p.PollId == x.PollId).ToList().Count(),
                //        //OptionPoll = objbd.PollResults.Where(p => p.ResultId == y.PollOptionId).ToList().Count(),
                //        //Percentage=(y.OptionPoll  * 100) /y.TotalPoll,

                //    })
                //}).ToList();
                if (list.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["PollList"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("PollListDetail")]
        [HttpPost]
        public HttpResponseMessage PollListDetail()
        {

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long PollId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["PollId"]);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            try
            {
                var list1 = objbd.Polls.Where(x=>x.PollId==PollId).Select(x => new
                {
                    PollId = x.PollId,
                    Title = x.Title,
                    Image = x.Image == "" ? null : PollingURL + x.Image,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate,
                    CreatedByUserId = x.CreatedBy,
                    CreatedByUserName = objbd.TblUsers.Where(y => y.UserId == x.CreatedBy).Select(y => y.Name).FirstOrDefault(),
                    VoteLastDate = x.VoteLastDate,
                    IsActive = x.IsActive,
                   
                }).ToList();
                var list = list1.Select(x => new
                {
                    PollId = x.PollId,
                    Title = x.Title,
                    Image = x.Image,
                    Description = x.Description,
                    CreatedDate = Convert.ToDateTime(x.CreatedDate).ToShortDateString(),
                    CreatedByUserId = x.CreatedByUserId,
                    CreatedByUserName = x.CreatedByUserName,
                    VoteLastDate = Convert.ToDateTime(x.VoteLastDate).ToShortDateString(),
                    IsActive = x.IsActive,
                  
                }).ToList();

                var Result = objbd.PollResults.Where(x => x.PollId == PollId).ToList();

                var choice = objbd.PollOptions.Where(x => x.PollId == PollId).ToList();
                var choiceList = choice.Select(x => new
                {
                    PollId = x.PollId,
                    PollOptionId = x.PollOptionId,
                    Options = x.Options,
                    ToltalPoll = objbd.PollResults.Where(y => y.PollId == x.PollId).ToList().Count(),
                    OptionPoll = objbd.PollResults.Where(y => y.ResultId == x.PollOptionId).ToList().Count(),
                }).ToList();


                var choiceLists = choiceList.Select(x => new
                {
                    PollId = x.PollId,
                    PollOptionId = x.PollOptionId,
                    Options = x.Options,
                    Percentage = x.OptionPoll == 0 ? 0 : (x.OptionPoll * 100) / x.ToltalPoll,
                    //ToltalPoll = x.ToltalPoll,
                    //OptionPoll =x.OptionPoll,
                }).ToList();

              

                if (list.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["PollDetail"] = list.ToArray();
                    Value["Choice"] = choiceLists.ToArray();
                  
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("AddPoll")]
        [HttpPost]
        public HttpResponseMessage AddPoll()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            string Title = System.Web.HttpContext.Current.Request.Form["Title"];
            string Description = System.Web.HttpContext.Current.Request.Form["Description"];
            string VoteLastDate = System.Web.HttpContext.Current.Request.Form["VoteLastDate"];
            var File = System.Web.HttpContext.Current.Request.Files["File"];
            string AnswarList = System.Web.HttpContext.Current.Request.Form["AnswarList"];
            string Answer = System.Web.HttpContext.Current.Request.Form["Answer"];
            string ReciepientList = System.Web.HttpContext.Current.Request.Form["ReciepientList"];
            PollBAL catBLL = new PollBAL();
            string strFileName = "";
            string path = "";
            Random rnd = new Random();
            try
            {
                long RollId = objbd.TblUsers.Where(x => x.UserId == UserId).Select(x => x.RoleId).SingleOrDefault();            
                 var roll = objbd.RoleAssignments.Where(x => x.ModuleId == 5 & x.RoleId == RollId).Select(x => x.DeleteRecord).SingleOrDefault();
                 if (Convert.ToBoolean(roll) == true)
                 {
                     long res = 0;
                     if (File != null)
                     {
                         strFileName = "UserImg_" + rnd.Next(100, 100000000) + "." + File.FileName.Split('.')[1].ToString();
                         path = System.Web.HttpContext.Current.Server.MapPath("~/Images/Polling/" + strFileName);
                         File.SaveAs(path);
                     }
                     res = new PollBAL { }.AddEditPoll(new PollModel
                     {
                         Title = Title,
                         Description = Description,
                         VoteLastDate = Convert.ToDateTime(VoteLastDate),
                         Image = strFileName,
                         CreatedDate = Date,
                         CreatedBy = UserId,
                         IsActive = true,

                     });

                     //assign Agent
                     string[] ReciepientLists = ReciepientList.Split(new string[] { "#@" }, StringSplitOptions.None);
                     foreach (var item in ReciepientLists)
                     {
                         new AssignPollingAgentBAL { }.AddEditAssignPollingAgent(new AssignPollingAgentModel
                         {
                             AgentId = Convert.ToInt32(item.Trim()),
                             PollId = res,

                         });
                     }


                     int i = 0;
                     //string[] AnswarLists = AnswarList.Split('#').Select(sValue => sValue.Trim()).ToArray();
                     string[] AnswarLists = AnswarList.Split(new string[] { "#@" }, StringSplitOptions.None);
                     //string[] Answers = Answer.Split(',').Select(sValue => sValue.Trim()).ToArray();
                     foreach (var item1 in AnswarLists)
                     {
                         if (AnswarLists[i].Trim() != "")
                         {
                             if (AnswarLists[i].Trim() == Answer.Trim())
                             {
                                 new PollOptionBAL { }.AddEditPollOption(new PollOptionModel
                                 {
                                     Options = AnswarLists[i].Trim(),
                                     PollId = res,
                                     IsTrue = true,

                                 });
                             }
                             else
                             {
                                 new PollOptionBAL { }.AddEditPollOption(new PollOptionModel
                                 {
                                     Options = AnswarLists[i].Trim(),
                                     PollId = res,
                                     IsTrue = false,

                                 });
                             }

                         }
                         i++;
                     }
                     if (res != 0)
                     {
                         Value["result"] = "TRUE";
                     }
                     else
                     {
                         Value["result"] = "FALSE";
                     }
                 }
                 else
                 {
                     Value["result"] = "FALSE";
                 }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

         public class polloptioncls
         {
             public Nullable<long> PollOptionId { get; set; }
             public string Option { get; set; }
             public string Percentage { get; set; }
         }

        [Route("SubmitVoting")]
        [HttpPost]
         public HttpResponseMessage SubmitVoting()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long PollId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["PollId"]);
            long OptionId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["OptionId"]);
            try
            {
                var list = objbd.PollResults.Where(x => x.AgentId == UserId & x.PollId == PollId).ToList();
                if (list.Count == 0)
                {
                    PollResult obj = new PollResult
                    {
                        PollId = PollId,
                        AgentId = UserId,
                        ResultId = OptionId,
                    };
                    objbd.PollResults.Add(obj);
                    objbd.SaveChanges();
                    long res = obj.Id;
                    if (res != 0)
                    {
                        var Result = objbd.PollResults.Where(x => x.PollId == PollId).ToList();
                        decimal count = Convert.ToDecimal(Result.Count());
                        decimal percentegperhade = 100 / count;
                        List<polloptioncls> poll = new List<polloptioncls>();
                        foreach(var item in Result)
                        {
                            var hascount = poll.Where(x => x.PollOptionId == item.ResultId).Count();
                            if(hascount==0)
                            {
                                Nullable<long> polloptionId = item.ResultId;
                                string option = objbd.PollOptions.Where(x => x.PollOptionId == polloptionId).Select(x => x.Options).SingleOrDefault(); ;
                                long vote = objbd.PollResults.Where(x => x.PollId == PollId & x.ResultId == polloptionId).Count();
                                decimal votepersenteg = percentegperhade * vote;
                                poll.Add(new polloptioncls { PollOptionId = polloptionId, Option = option, Percentage = System.Math.Round(votepersenteg, 2).ToString() + " " + "%" });
                            }
                            
                        }
                        Value["result"] = "TRUE";
                        Value["Pollesult"] = poll.ToArray();
                        
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
                }
                else
                {

                    var Result = objbd.PollResults.Where(x => x.PollId == PollId).ToList();
                    decimal count = Convert.ToDecimal(Result.Count());
                    decimal percentegperhade = 100 / count;
                    List<polloptioncls> poll = new List<polloptioncls>();
                    foreach (var item in Result)
                    {
                       var hascount = poll.Where(x => x.PollOptionId == item.ResultId).Count();
                            if(hascount==0)
                            {
                                Nullable<long> polloptionId = item.ResultId;
                                string option = objbd.PollOptions.Where(x => x.PollOptionId == polloptionId).Select(x => x.Options).SingleOrDefault(); ;
                                long vote = objbd.PollResults.Where(x => x.PollId == PollId & x.ResultId == polloptionId).Count();
                                decimal votepersenteg = percentegperhade * vote;
                                poll.Add(new polloptioncls { PollOptionId = polloptionId, Option = option, Percentage = System.Math.Round(votepersenteg, 2).ToString() + " " + "%" });
                            }
                    }
                    Value["result"] = "FALSE";
                    Value["message"] = "Exist";
                    Value["Pollesult"] = poll.ToArray();
                   
                }                
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("DeletePoll")]
        [HttpPost]
        public HttpResponseMessage DeletePoll()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long PollId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["PollId"]);
            long UserID = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                long RollId = objbd.TblUsers.Where(x => x.UserId == UserID).Select(x => x.RoleId).SingleOrDefault();
                var roll = objbd.RoleAssignments.Where(x => x.ModuleId == 5 & x.RoleId == RollId).Select(x => x.DeleteRecord).SingleOrDefault();
                if (Convert.ToBoolean(roll) == true)
                {
                    var list = objbd.PollResults.Where(x => x.PollId == PollId).ToList();
                   
                    if (list.Count == 0)
                    {

                        //Delete PollOption
                        objbd.PollOptions.Where(p => p.PollId == PollId).ToList().ForEach(p => objbd.PollOptions.Remove(p));
                        objbd.SaveChanges();


                        //delete polling
                        var pri1 = (from ok in objbd.Polls where PollId == ok.PollId select ok).Single();
                        objbd.Polls.Attach(pri1);
                        objbd.Polls.Remove(pri1);
                        objbd.SaveChanges();
                        if (pri1 != null)
                        {
                            Value["result"] = "TRUE";
                        }
                        else
                        {
                            Value["result"] = "FALSE";
                        }
                    }
                    else
                    {
                        Value["result"] = "You Can't delete . " + list.Count + " user already polled.";
                    }
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }
     
    
        [Route("AddAnnouncement")]
        [HttpPost]
        public HttpResponseMessage AddAnnouncement()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            string Title = System.Web.HttpContext.Current.Request.Form["Title"];
            string Description = System.Web.HttpContext.Current.Request.Form["Description"];
            string PublishDate = System.Web.HttpContext.Current.Request.Form["PublishDate"];
            string ExpireDate = System.Web.HttpContext.Current.Request.Form["ExpireDate"];
            string ReciepientList = System.Web.HttpContext.Current.Request.Form["ReciepientList"];
            var file = System.Web.HttpContext.Current.Request.Files["File"];
            CustomMethods.ValidateRoles("Announcement");
            Random rnd = new Random();
            try
            {
                long RollId = objbd.TblUsers.Where(x => x.UserId == UserId).Select(x => x.RoleId).SingleOrDefault();
                var roll = objbd.RoleAssignments.Where(x => x.ModuleId == 1 & x.RoleId==RollId).SingleOrDefault();
                if (roll.AddRecord == true)
                {
                    string strFileName = "";
                    string path = "";
                    long res = 0;
                    if (file != null)
                    {
                        strFileName = "AnnouncementImg_" + rnd.Next(100, 100000000) + "." + file.FileName.Split('.')[1].ToString();
                        path = System.Web.HttpContext.Current.Server.MapPath("~/Images/Announcements/" + strFileName);
                        file.SaveAs(path);
                    }
                    res = new AnnouncementBAL { }.AddEditAnnouncement(new AnnouncementModel
                    {
                        Title = Title,
                        Description = Description,
                        PublishDate = Convert.ToDateTime(PublishDate),
                        ExpireDate = Convert.ToDateTime(ExpireDate),
                        Image = strFileName,
                        CreatedBy = UserId,

                    });
                    if (res != 0)
                    {
                        string[] ReciepientLists = ReciepientList.Split(new string[] { "#@" }, StringSplitOptions.None);
                        foreach (var item in ReciepientLists)
                        {
                            new AssignAnnouncementBAL { }.AddEditAssignAnnouncement(new AssignAnnouncementModel
                            {
                                UserId =Convert.ToInt32(item.Trim()),
                                AnnouncementId = res,
                                
                            });
                        }


                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
                }
                else
                {
                    Value["result"] = "FALSE";
                }
                
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }
        [Route("Announcement")]
        [HttpPost]
        public HttpResponseMessage Announcement()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var list1 = objbd.AssignAnnouncements.Where(x=>x.UserId==UserId).Select(x => new
                {
                    Id = x.Announcement.AnnouncementId,
                    Title = x.Announcement.Title,
                    Image = x.Announcement.Image,
                    Desctiption = x.Announcement.Description,
                    PublishDate = x.Announcement.PublishDate,
                    ExpireDate = x.Announcement.ExpireDate,
                    CreatedByUserId = x.Announcement.CreatedBy,
                    CreatedByUserName = objbd.TblUsers.Where(y => y.UserId == x.Announcement.CreatedBy).Select(y => y.Name).FirstOrDefault(),
                }).OrderByDescending(x => x.Id).ToList();
                var list = list1.Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    Image = x.Image == "" ? null : AnnouncementsURL + x.Image,
                    Desctiption = x.Desctiption,
                    PublishDate = Convert.ToDateTime(x.PublishDate).ToShortDateString(),
                    ExpireDate = Convert.ToDateTime(x.ExpireDate).ToShortDateString(),
                    CreatedByUserId = x.CreatedByUserId,
                    CreatedByUserName = x.CreatedByUserName,


                }).OrderByDescending(x => x.Id).ToList();
                if (list.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["Announcement"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }
        [Route("DeleteAnnouncement")]
        [HttpPost]
        public HttpResponseMessage DeleteAnnouncement()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long DocumentId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["AnnouncementId "]);
            long UserID = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                long RollId = objbd.TblUsers.Where(x => x.UserId == UserID).Select(x => x.RoleId).SingleOrDefault();
                  var roll = objbd.RoleAssignments.Where(x => x.ModuleId == 1 & x.RoleId==RollId).Select(x=>x.DeleteRecord).SingleOrDefault();
                  if (Convert.ToBoolean(roll) == true)
                  {
                      var pri1 = (from ok in objbd.Announcements where DocumentId == ok.AnnouncementId select ok).Single();
                      objbd.Announcements.Attach(pri1);
                      objbd.Announcements.Remove(pri1);
                      objbd.SaveChanges();
                      if (pri1 != null)
                      {
                          Value["result"] = "TRUE";
                      }
                      else
                      {
                          Value["result"] = "FALSE";
                      }
                  }
                  else
                  {                        
                      Value["result"] = "FALSE";
                  }
              
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("AgentListManagerWise")]
        [HttpPost]
        public HttpResponseMessage AgentListManagerWise()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var list = objbd.TblUsers.Where(x=>x.ManagerID==UserId).Select(x => new
                {
                    AgentId = x.UserId,
                    AgentName = x.Name,
                    ManagerId = x.ManagerID
                }).OrderByDescending(x => x.AgentId).ToList();
                if (list.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["AgentListManagerWise"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("ManagerAddSubmission")]
        [HttpPost]
        public HttpResponseMessage ManagerAddSubmission()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            DateTime Date = Convert.ToDateTime(System.Web.HttpContext.Current.Request.Form["Date"]);

            string AgentId = System.Web.HttpContext.Current.Request.Form["AgentId"];
            long ManagerID = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["ManagerID"]);
            string Amount = System.Web.HttpContext.Current.Request.Form["Amount"];
            long res = 0;
   
            try
            {
                long RollId = objbd.TblUsers.Where(x => x.UserId == ManagerID).Select(x => x.RoleId).SingleOrDefault();
                var roll = objbd.RoleAssignments.Where(x => x.ModuleId == 10 & x.RoleId == RollId).Select(x => x.AddRecord).SingleOrDefault();
                if (Convert.ToBoolean(roll) == true)
                {
                    string ReciepientList = System.Web.HttpContext.Current.Request.Form["ReciepientList"];

                    string[] AgentIds = AgentId.Split(new string[] { "#@" }, StringSplitOptions.None);
                    //string[] AgentIds = AgentId.Split(',').Select(sValue => sValue.Trim()).ToArray();
                    string[] Amounts = Amount.Split(new string[] { "#@" }, StringSplitOptions.None);
                    //string[] Amounts = Amount.Split(',').Select(sValue => sValue.Trim()).ToArray();
                 
 
                        int i = 0;
                        foreach (var item in AgentIds)
                        {
                            var agnt = objbd.ManagerdistributeAmounts.Where(x => x.Date.Date == Date.Date && x.AgentId == Convert.ToInt32(item)).SingleOrDefault();
                            res= new ManagerdistributeAmountBAL { }.AddEditManagerdistributeAmount(new ManagerdistributeAmountModel
                            {
                                ID = agnt.ID,
                                AgentId = Convert.ToInt32(item),
                                ManagerId =ManagerID,
                                Amount = Convert.ToDecimal(Amounts[i].Trim()),
                                Date = Date

                            });
                            i++;
                        }

                        Value["result"] = "True";

                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        //[Route("AddAgentschedule")]
        //[HttpPost]
        //public HttpResponseMessage AddAgentschedule()
        //{
        //    Dictionary<string, object> Value = new Dictionary<string, object>();
        //    HttpResponseMessage response = null;

        //    long AgentId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["AgentId"]);
        //    string Location = System.Web.HttpContext.Current.Request.Form["Location"];
        //    string Date = System.Web.HttpContext.Current.Request.Form["Date"];
        //    string StartTime = System.Web.HttpContext.Current.Request.Form["StartTime"];
        //    string Endtime = System.Web.HttpContext.Current.Request.Form["Endtime"];
        //    string Cost = System.Web.HttpContext.Current.Request.Form["Cost"];
        //    string Description = System.Web.HttpContext.Current.Request.Form["Description"];

        //    try
        //    {
        //        long RollId = objbd.TblUsers.Where(x => x.UserId == AgentId).Select(x => x.RoleId).SingleOrDefault();
        //        var roll = objbd.RoleAssignments.Where(x => x.ModuleId == 3 & x.RoleId == RollId).Select(x => x.AddRecord).SingleOrDefault();
        //        if (Convert.ToBoolean(roll) == true)
        //        {
        //            Duty objDuty = new Duty
        //            {
        //                Location = Location,
        //                Description = Description,
        //                DutyDate = Convert.ToDateTime(Date),
        //                StartTime = StartTime,
        //                EndTime = Endtime,
        //                Cost = Convert.ToDecimal(Cost),
        //                AgentId = AgentId,
        //            };
        //            objbd.Duties.Add(objDuty);
        //            objbd.SaveChanges();
        //            long res = objDuty.DutyId;
        //            if (res != null)
        //            {
        //                Value["result"] = "TRUE";
        //            }
        //            else
        //            {
        //                Value["result"] = "FALSE";
        //            }
        //        }
        //        else
        //        {
        //            Value["result"] = "FALSE";
        //        }
        //    }
        //    catch
        //    {
        //        Value["result"] = "FALSE";
        //    }
        //    response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
        //    return response;
        //}

        [Route("AgentscheduleList")]
        [HttpPost]
        public HttpResponseMessage AgentscheduleList()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var list = objbd.AssignDuties.Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Duty.Location,
                    RoadshowCode = x.Duty.RoadshowCode,
                    DateFrom = x.Duty.DateFrom,
                    DateTo = x.Duty.DateTo,
                    Description = x.Duty.Description,
                    Cost = x.Duty.Cost,
                    AgentId = x.AgentId,
                    AgentName = x.TblUser.Name
                }).OrderByDescending(x => x.AgentId).ToList();
                if (list.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["Agentschedule"] = list.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }


        [Route("GetTeamAgentList")]
        [HttpPost]
        public HttpResponseMessage GetTeamAgentList()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["ManagerId"]);
            try
            {
                var sd = objbd.TblUsers.Where(x => x.ManagerID == UserId).ToList();
                var adminDet = sd.Select(x => new
                {
                    UserId = x.UserId,
                    RollId = x.RoleId,
                    Name = x.Name,
                    Gender = x.Gender,
                    DOB = x.DOB,
                    Email = x.EmailId,
                    Mobile = x.Mobile,
                    Photo = ImageURL + x.Photo,
                    AgentCode = x.AgentCode,
                    Designation = x.Role.RoleName,
                    DateOfJoined = x.JoinDate,
                    AwardList = objbd.Awards.Where(y => y.UserId == x.UserId).Select(y => new
                    {
                        AwardTitle = y.Title,
                    }).ToList(),

                }).ToList();

                if (adminDet.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["TeamAgentList"] = adminDet.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }
        [Route("DutyList")]
        [HttpPost]
        public HttpResponseMessage DutyList()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = Convert.ToDateTime(System.Web.HttpContext.Current.Request.Form["Date"]);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var dut = objbd.Duties.ToList();
                var list = dut.Where(x => (x.DateFrom.Year == Date.Year && x.DateFrom.Month == Date.Month) || x.DateTo.Year == Date.Year && x.DateTo.Month == Date.Month).Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.Cost,
                    AgentList = objbd.AssignDuties.Where(y => y.DutyId == x.DutyId).Select(y => new
                                    {
                                        AssignDutyId = y.AssignDutyId,
                                        AgentId = y.AgentId,
                                        Name = y.TblUser.Name,
                                        Photo = ImageURL + y.TblUser.Photo,
                                        user_available = y.UserAvailable,
                                        trade_message = y.trade_message,
                                        //meRequested = objbd.RequestTrades.Where(t => t.AssignDuty.DutyId == x.DutyId && t.RequestAgentId == UserId).ToList().Count() == 0 ? false : true,
                                    }).ToList(),

                }).ToList();
                var lst = list.Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.TotalCost,
                    AgentList = x.AgentList.Select(y => new
                    {
                        //AssignDutyId = y.AssignDutyId,
                        AgentId = y.AgentId,
                        Name = y.Name,
                        Photo = y.Photo,
                        user_available = y.user_available,
                        trade_message = y.trade_message,
                        meRequested = objbd.RequestTrades.Where(t => t.AssignDutyId == y.AssignDutyId && t.RequestAgentId == UserId).ToList().Count() == 0 ? false : true,
                        my_message = objbd.RequestTrades.Where(t => t.AssignDutyId == y.AssignDutyId && t.RequestAgentId == UserId).Select(t => t.Message).SingleOrDefault() == null ? null : objbd.RequestTrades.Where(t => t.AssignDutyId == y.AssignDutyId && t.RequestAgentId == UserId).Select(t => t.Message).SingleOrDefault(),
                    }).ToList(),

                }).ToList();
                if (lst.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["DutyList"] = lst.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("FreeSlotList")]
        [HttpPost]
        public HttpResponseMessage FreeSlotList()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var dut = objbd.Duties.ToList();
                var list = dut.Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.Cost,
                    AgentList = objbd.AssignDuties.Where(y=>y.DutyId==x.DutyId).Select(y => new
                    {
                        AssignDutyId = y.AssignDutyId,
                        AgentId = y.AgentId,
                        Name = y.TblUser.Name,
                        Photo = ImageURL + y.TblUser.Photo,
                        user_available = y.UserAvailable,
                        trade_message = y.trade_message,
                       
                    }).ToList(),

                }).ToList();
                var lst = list.Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.TotalCost,
                    AgentList = x.AgentList.Select(y => new
                    {
                        AgentId = y.AgentId,
                        Name = y.Name,
                        Photo = y.Photo,
                        user_available = y.user_available,
                        trade_message = y.trade_message,
                        meRequested = objbd.RequestTrades.Where(t => t.AssignDutyId == y.AssignDutyId && t.RequestAgentId == UserId).ToList().Count() == 0 ? false : true,
                        my_message = objbd.RequestTrades.Where(t => t.AssignDutyId == y.AssignDutyId && t.RequestAgentId == UserId).Select(t => t.Message).SingleOrDefault() == null ? null : objbd.RequestTrades.Where(t => t.AssignDutyId == y.AssignDutyId && t.RequestAgentId == UserId).Select(t => t.Message).SingleOrDefault(),
                        Accept = objbd.RequestTrades.Where(t => t.AssignDutyId == y.AssignDutyId && t.RequestAgentId == UserId && t.Accept==true).Select(t => t.Accept).SingleOrDefault() ,
                    }).ToList(),

                }).ToList();
                var SlotList = lst.Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.TotalCost,
                    AgentList = x.AgentList.Where(m => m.user_available == false && m.trade_message != null).Select(y => new
                    {
                        AgentId = y.AgentId,
                        Name = y.Name,
                        Photo = y.Photo,
                        user_available = y.user_available,
                        trade_message = y.trade_message,
                        meRequested = y.meRequested,
                        my_message = y.my_message,
                        Accept = y.Accept,
                    }).ToList(),
                    agentcount = x.AgentList.Where(m => m.user_available == false && m.trade_message != null).Select(y => new
                    {
                        AgentId = y.AgentId,
                        Name = y.Name,
                        Photo = y.Photo,
                        user_available = y.user_available,
                        trade_message = y.trade_message,
                        meRequested = y.meRequested,
                        my_message = y.my_message,
                        //Accept = y.Accept,
                    }).ToList().Count(),
                }).ToList();

                var FreeSlotList = SlotList.Where(x=>x.agentcount > 0).Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.TotalCost,
                    AgentList = x.AgentList.Select(y => new
                    {
                        AgentId = y.AgentId,
                        Name = y.Name,
                        Photo = y.Photo,
                        user_available = y.user_available,
                        trade_message = y.trade_message,
                        meRequested = y.meRequested,
                        my_message = y.my_message,
                        Accept = y.Accept,
                    }).ToList(),
                }).ToList();
                if (FreeSlotList.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["FreeSlotList"] = FreeSlotList.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("MyRequestSlotList")]
        [HttpPost]
        public HttpResponseMessage MyRequestSlotList()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var dut = objbd.Duties.ToList();
                var list = dut.Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.Cost,
                    AgentList = objbd.RequestTrades.Where(y => y.AssignDuty.DutyId == x.DutyId && y.RequestAgentId==UserId).Select(y => new
                    {
                        AssignDutyId = y.AssignDutyId,
                        AgentId = y.AssignDuty.AgentId,
                        Name = y.TblUser.Name,
                        Photo = ImageURL + y.TblUser.Photo,
                        Message = y.Message,
                        trade_message = y.AssignDuty.trade_message,
                        UserAvailable = y.AssignDuty.UserAvailable,
                    }).ToList(),

                }).ToList();

                var MyRequestSlotList = list.Where(x => x.AgentList.Count > 0).Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.TotalCost,
                    AgentList = x.AgentList.Select(y => new
                    {
                        //AssignDutyId = y.AssignDutyId,
                        AgentId = y.AgentId,
                        Name = y.Name,
                        Photo = y.Photo,
                        Message = y.Message,
                        trade_message = y.trade_message,
                        UserAvailable = y.UserAvailable,
                    }).ToList(),

                }).ToList();
                if (MyRequestSlotList.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["MyRequestSlotList"] = MyRequestSlotList.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("OtherRequestSlotList")]
        [HttpPost]
        public HttpResponseMessage OtherRequestSlotList()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {
                var dut = objbd.Duties.ToList();
                var list = dut.Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.Cost,
                    AgentList = objbd.RequestTrades.Where(y => y.AssignDuty.DutyId == x.DutyId && y.AssignDuty.AgentId == UserId ).Select(y => new
                    {
                        AssignDutyId = y.AssignDutyId,
                        AgentId = y.RequestAgentId,
                        Name = y.TblUser.Name,
                        Photo = ImageURL + y.TblUser.Photo,
                        Message = y.Message,
                    }).ToList(),

                }).ToList();

                var OtherRequestSlotList = list.Where(x=>x.AgentList.Count > 0).Select(x => new
                {
                    DutyId = x.DutyId,
                    Location = x.Location,
                    RoadshowCode = x.RoadshowCode,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Description = x.Description,
                    TotalCost = x.TotalCost,
                    AgentList = x.AgentList.Select(y => new
                    {
                        //AssignDutyId = y.AssignDutyId,
                        AgentId = y.AgentId,
                        Name = y.Name,
                        Photo = y.Photo,
                        Message = y.Message,
                    }).ToList(),

                }).ToList();
                if (OtherRequestSlotList.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["OtherRequestSlotList"] = OtherRequestSlotList.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("SetMyTrade")]
        [HttpPost]
        public HttpResponseMessage SeMyTrade()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DutyId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DutyId"]);
            string message = System.Web.HttpContext.Current.Request.Form["message"];
            try
            {
                //var chk = objbd.AssignDuties.Where(x => x.DutyId == DutyId && x.AgentId == UserId).SingleOrDefault();
                //if (chk == null)
                //{
                    //AssignDuty objAnnouncement = new AssignDuty
                    //{
                    //    AgentId = UserId,
                    //    DutyId = DutyId,
                    //    trade_message = message,
                    //    UserAvailable = false,
                    //};
                    //objbd.AssignDuties.Add(objAnnouncement);
                    //objbd.SaveChanges();
                    //long res = objAnnouncement.AssignDutyId;
                    var objAnnouncement = objbd.AssignDuties.Where(x => x.AgentId == UserId && x.DutyId == DutyId).SingleOrDefault();
                    if (objAnnouncement != null)
                    {
                        objAnnouncement.UserAvailable = false;
                        objAnnouncement.trade_message = message;
                        objbd.SaveChanges();
                        //long res = objAnnouncement.AssignDutyId;
                    }
                    long res = objAnnouncement.AssignDutyId;

                    if (res != 0)
                    {
                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
                //}
                //else {
                //    Value["Message"] = "Already set trade";
                //    Value["result"] = "FALSE";
                //}

           

            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("CancelMyTrade")]
        [HttpPost]
        public HttpResponseMessage CancelMyTrade()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DutyId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DutyId"]);
            try
            {

                var objAnnouncement = objbd.AssignDuties.Where(x=>x.AgentId==UserId && x.DutyId==DutyId).SingleOrDefault();
                if (objAnnouncement != null)
                {
                    objAnnouncement.UserAvailable = true;
                    objAnnouncement.trade_message = null;
                    objbd.SaveChanges();
                }

            
                  
               Value["result"] = "TRUE";
                 
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("RequestToReplace ")]
        [HttpPost]
        public HttpResponseMessage Requesttoreplace()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DutyId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DutyId"]);
            long other_userid = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["other_userid"]);
            string message = System.Web.HttpContext.Current.Request.Form["message"];
            try
            {
                var chk = objbd.AssignDuties.Where(x => x.DutyId == DutyId && x.AgentId == UserId).SingleOrDefault();
                if (chk != null)
                {
                    RequestTrade objAnnouncement = new RequestTrade
                    {
                        RequestAgentId = other_userid,
                        AssignDutyId = chk.AssignDutyId,
                        Message = message,
                       
                    };
                    objbd.RequestTrades.Add(objAnnouncement);
                    objbd.SaveChanges();
                    long res = objAnnouncement.RequestTradeId;

                    if (res != 0)
                    {
                        Value["result"] = "TRUE";
                    }
                    else
                    {
                        Value["result"] = "FALSE";
                    }
                }
                else
                {
                    Value["Message"] = "Already set trade";
                    Value["result"] = "FALSE";
                }



            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("CancelMyTradeOtherUserSlot")]
        [HttpPost]
        public HttpResponseMessage CancelMyTradeOtherUserSlot()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DutyId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DutyId"]);
            long other_userid = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["other_userid"]);
            try
            {
                var chk = objbd.AssignDuties.Where(x => x.AgentId == UserId && x.DutyId == DutyId).SingleOrDefault();

                var crt = objbd.RequestTrades.Where(x => x.AssignDutyId == chk.AssignDutyId && x.RequestAgentId==other_userid).SingleOrDefault();
                objbd.RequestTrades.Remove(crt);
                objbd.SaveChanges();
                Value["result"] = "TRUE";

            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("DeclineOtherUserRequest")]
        [HttpPost]
        public HttpResponseMessage DeclineOtherUserRequest()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DutyId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DutyId"]);
            long other_userid = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["other_userid"]);
            try
            {
                var chk = objbd.AssignDuties.Where(x => x.AgentId == UserId && x.DutyId == DutyId).SingleOrDefault();

                var crt = objbd.RequestTrades.Where(x => x.AssignDutyId == chk.AssignDutyId && x.RequestAgentId == other_userid).SingleOrDefault();
                objbd.RequestTrades.Remove(crt);
                objbd.SaveChanges();
                Value["result"] = "TRUE";

            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("AcceptOtherUserRequest")]
        [HttpPost]
        public HttpResponseMessage AcceptOtherUserRequest()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DutyId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DutyId"]);
            long other_userid = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["other_userid"]);
            try
            {
                var chk = objbd.AssignDuties.Where(x => x.AgentId == UserId && x.DutyId == DutyId).SingleOrDefault();

                var chkAc = objbd.RequestTrades.Where(x => x.AssignDutyId == chk.AssignDutyId).SingleOrDefault();
                if (chkAc == null)
                {
                    var objAnnouncement = objbd.RequestTrades.Where(x => x.AssignDutyId == chk.AssignDutyId && x.RequestAgentId == other_userid).SingleOrDefault();
                    if (objAnnouncement != null)
                    {
                        objAnnouncement.Accept = true;

                        objbd.SaveChanges();
                    }

                    Value["result"] = "TRUE";
                }
                else 
                {
                    Value["Message"] = "Already accepted User";
                    Value["result"] = "False";
                }


            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("SetRTS")]
        [HttpPost]
        public HttpResponseMessage SetRTS()
        {
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;

            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            long DutyId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["DutyId"]);
            string RTS = System.Web.HttpContext.Current.Request.Form["RTS"];
            try
            {
                var chk = objbd.AssignDuties.Where(x => x.AgentId == UserId && x.DutyId == DutyId).SingleOrDefault();


                    var objAnnouncement = objbd.AssignDuties.Where(x => x.AssignDutyId == chk.AssignDutyId).SingleOrDefault();
                    if (objAnnouncement != null)
                    {
                        objAnnouncement.RTS = RTS;
                        objbd.SaveChanges();
                    }

                    Value["result"] = "TRUE";
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("MTDSubmission")]
        [HttpPost]
        public HttpResponseMessage MTDSubmission()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            //DateTime Fromdt = Convert.ToDateTime(System.Web.HttpContext.Current.Request.Form["Date"]);

            DateTime Dtless = Date.AddMonths(-1);
            int Rank = 0;
            //long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {

                var list1 = objbd.ManagerdistributeAmounts.Select(x => new
                {
                    ID = x.ID,
                    Date = x.Date,
                    Name = x.TblUser.Name,
                    Image = x.TblUser.Photo,
                    UserId = x.AgentId,
                    Amount = x.Amount,
                }).ToList();

                var list = list1.Where(x => x.Date.Year == Date.Year).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.Name,
                    Image = x.Image,
                    Day1 = list1.Where(y => y.UserId == x.UserId && x.Date.Month < Date.Month && x.Date.Year == Date.Year).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.UserId == x.UserId && x.Date.Month < Date.Month && x.Date.Year == Date.Year).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Month == Date.Month && y.UserId == x.UserId && x.Date.Year == Date.Year).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.Date.Month == Date.Month && y.UserId == x.UserId && x.Date.Year == Date.Year).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                var finallist = list.Where(x => x.Day1 != null).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.AgentName,
                    Image = ImageURL + x.Image,
                    Year = x.Day1,
                    Month = x.Day,
                    total = x.Day + x.Day1,
                }).Distinct().OrderByDescending(x => x.total).Take(6).ToList();
                var fslist = list.Where(x => x.Day1 != null).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.AgentName,
                    Image = ImageURL + x.Image,
                    Year = x.Day1,
                    Month = x.Day,
                    total = x.Day + x.Day1,
                }).Distinct().OrderByDescending(x => x.total).ToList();
                int id = 1;
                foreach (var item in fslist)
                {
                    if (item.UserId == UserId)
                    {
                        Rank = id;
                        break;
                    }
                    id++;
                }

                if (finallist.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["MTD"] = finallist.ToArray();
                    Value["Rank"] = Rank;
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("DailySubmission")]
        [HttpPost]
        public HttpResponseMessage DailySubmission()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            //DateTime FromDT = Convert.ToDateTime(System.Web.HttpContext.Current.Request.Form["FromDate"]);
            //DateTime ToDate = Convert.ToDateTime(System.Web.HttpContext.Current.Request.Form["ToDate"]);
            DateTime Dtless = Date.AddDays(-1);
            //long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {

                var list1 = objbd.ManagerdistributeAmounts.Select(x => new
                {
                    ID = x.ID,
                    Date = x.Date,
                    Name = x.TblUser.Name,
                    Image = x.TblUser.Photo,
                    UserId = x.AgentId,
                    Amount = x.Amount,
                }).ToList();

                var list = list1.Where(x => x.Date.Date < Date.Date).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.Name,
                    Image = x.Image,
                    Day1 = list1.Where(y => y.UserId == x.UserId).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.UserId == x.UserId).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Date == Date.Date && y.UserId == x.UserId).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.Date.Date == Date.Date && y.UserId == x.UserId).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                var finallist = list.Where(x => x.Day1 != null).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.AgentName,
                    Image = ImageURL + x.Image,
                    AmountDay1 = x.Day1,
                    AmountDay = x.Day,
                    total = x.Day + x.Day1,
                }).Distinct().OrderByDescending(x => x.total).Take(6).ToList();

                var fslist = list.Where(x => x.Day1 != null).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.AgentName,
                    Image = ImageURL + x.Image,
                    Year = x.Day1,
                    Month = x.Day,
                    total = x.Day + x.Day1,
                }).Distinct().OrderByDescending(x => x.total).ToList();
                int Rank = 0;
                int id = 1;
                foreach (var item in fslist)
                {
                    if (item.UserId == UserId)
                    {
                        Rank = id;
                        break;
                    }
                    id++;
                }

                if (finallist.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["MTD"] = finallist.ToArray();
                    Value["Rank"] = Rank;
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("MTDInceptionChart")]
        [HttpPost]
        public HttpResponseMessage MTDInceptionChart()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            //DateTime Fromdt = Convert.ToDateTime(System.Web.HttpContext.Current.Request.Form["Date"]);
            //long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {

                var list1 = objbd.Inceptions.Select(x => new
                {
                    InceptionId = x.InceptionId,
                    Month = x.MonthFrom,
                    Year = x.YearFrom,
                    AgentId = x.AgentId,
                    Name = x.TblUser.Name,
                    MTD_WAPI = x.MTD_WAPI,
                    YTD_WAPI = x.YTD_WAPI
                }).ToList();

                var list = list1.Where(x => x.Year == Date.Year).Select(x => new
                {
                    AgentName = x.Name,
                    AgentId = x.AgentId,
                    //Date = x.Date,
                    Year = list1.Where(y => y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.YTD_WAPI)).SingleOrDefault(),
                    Month = list1.Where(y => y.Month == Date.Month && y.AgentId == x.AgentId).GroupBy(y => x.AgentId).Select(y => y.Sum(c => c.MTD_WAPI)).SingleOrDefault(),
                    //total = x.YTD_WAPI + x.MTD_WAPI,
                }).Distinct().ToList();

                var listt = list.Where(x => x.Year != null).Select(x => new
                {
                    AgentId = x.AgentId,
                    AgentName = x.AgentName,
                    Year = x.Year,
                    Month = x.Month,
                    total = x.Year + x.Month,
                }).Distinct().OrderByDescending(x => x.total).ToList();

                var listtf = list.Where(x => x.Year != null).Select(x => new
                {
                    AgentId = x.AgentId,
                    AgentName = x.AgentName,
                    Year = x.Year,
                    Month = x.Month,
                    total = x.Year + x.Month,
                }).Distinct().OrderByDescending(x => x.total).ToList();
                int Rank = 0;
                int id = 1;
                foreach (var item in listtf)
                {
                    if (item.AgentId == UserId)
                    {
                        Rank = id;
                        break;
                    }
                    id++;
                }
                if (listt.Count() > 0)
                {
                    Value["result"] = "TRUE";
                    Value["list"] = listt;
                    Value["Rank"] = Rank;
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("MTD")]
        [HttpPost]
        public HttpResponseMessage MTD()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            DateTime Dtless = Date.AddMonths(-1);
            //long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {

                var list1 = objbd.ManagerdistributeAmounts.Select(x => new
                {
                    ID = x.ID,
                    Date = x.Date,
                    Name = x.TblUser.Name,
                    Image = x.TblUser.Photo,
                    UserId = x.AgentId,
                    Amount = x.Amount,
                }).ToList();

                var list = list1.Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.Name,
                    Image = x.Image,
                    Day1 = list1.Where(y => y.UserId == x.UserId && x.Date.Month < Date.Month && x.Date.Year == Date.Year).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault() == null ? 0 : list1.Where(y => y.UserId == x.UserId && x.Date.Month < Date.Month).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Month == Date.Month && y.UserId == x.UserId && x.Date.Year == Date.Year).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                var finallist = list.Where(x => x.Day1 != 0).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.AgentName,
                    Image = ImageURL + x.Image,
                    Year = x.Day1,
                    Month = x.Day,
                    total = x.Day + x.Day1,
                }).Distinct().OrderByDescending(x => x.total).Take(6).ToList();

                if (finallist.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["MTD"] = finallist.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("Daily")]
        [HttpPost]
        public HttpResponseMessage Daily()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);

            //long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {

                var list1 = objbd.ManagerdistributeAmounts.Select(x => new
                {
                    ID = x.ID,
                    Date = x.Date,
                    Name = x.TblUser.Name,
                    Image = x.TblUser.Photo,
                    UserId = x.AgentId,
                    Amount = x.Amount,
                }).ToList();

                var list = list1.Where(x => x.Date.Month == Date.Month).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.Name,
                    Image = x.Image,
                    Day1 = list1.Where(y => x.Date.Date < Date.Date && y.UserId == x.UserId).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                    Day = list1.Where(y => y.Date.Date == Date.Date && y.UserId == x.UserId).GroupBy(y => x.UserId).Select(y => y.Sum(c => c.Amount)).SingleOrDefault(),
                }).Distinct().ToList();

                var finallist = list.Where(x => x.Day1 != null).Select(x => new
                {
                    UserId = x.UserId,
                    AgentName = x.AgentName,
                    Image = ImageURL + x.Image,
                    AmountDay1 = x.Day1,
                    AmountDay = x.Day,
                    total = x.Day + x.Day1,
                }).Distinct().OrderByDescending(x => x.total).Take(6).ToList();

                if (finallist.Count > 0)
                {
                    Value["result"] = "TRUE";
                    Value["MTD"] = finallist.ToArray();
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }

        [Route("TargetOrInception")]
        [HttpPost]
        public HttpResponseMessage TargetOrInception()
        {
            Dictionary<string, object> Value = new Dictionary<string, object>();
            HttpResponseMessage response = null;
            DateTime Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            decimal Target = 0;
            decimal Inception = 0;
            long UserId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["UserId"]);
            try
            {

                var Tget = objbd.TblUsers.Where(x => x.UserId == UserId).GroupBy(x => x.UserId).Select(x => new
                {
                    Target = x.Sum(c => c.AgentTarget) == null ? 0 : x.Sum(c => c.AgentTarget),
                }).Distinct().SingleOrDefault();
                if (Tget != null)
                {
                    Target = Convert.ToDecimal(Tget.Target);
                }

                var Ince = objbd.Inceptions.Where(x => x.AgentId == UserId).GroupBy(x => x.AgentId).Select(x => new
                {
                    Inception = x.Sum(c => c.YTD_WAPI) == null ? 0 : x.Sum(c => c.YTD_WAPI),
                }).Distinct().SingleOrDefault();
                if (Ince != null)
                {
                    Inception = Ince.Inception;
                }
              

                if (Target > 0)
                {
                    Value["result"] = "TRUE";
                    Value["Target"] = Target;
                    Value["Inception"] = Inception;
                }
                else
                {
                    Value["result"] = "FALSE";
                }
            }
            catch
            {
                Value["result"] = "FALSE";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Value, "application/json");
            return response;
        }


    }
}

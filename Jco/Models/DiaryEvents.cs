using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization; 
using JcoDAL;// << dont forget to add this for converting dates to localtime

namespace Jco.Models
{
    public class DiaryEvent
    {

        public long ID;
        public string Title;
        public int SomeImportantKeyID;
        public string StartDateString;
        public string EndDateString;
        public string StatusString;
        public string StatusColor;
        public string ClassName;

        JCOEntities objdb = new JCOEntities();

        //public static List<DiaryEvent> LoadAllAppointmentsInDateRange(double start, double end)
        //{
        //    var fromDate = ConvertFromUnixTimestamp(start);
        //    var toDate = ConvertFromUnixTimestamp(end);
        //    using (JCOEntities ent = new JCOEntities())
        //    {
        //        var rslt = ent.Duties.Where(s => s.DutyDate >= fromDate && s.DutyDate <= toDate);

        //        List<DiaryEvent> result = new List<DiaryEvent>();
        //        foreach (var item in rslt)
        //        {
        //            DiaryEvent rec = new DiaryEvent();
        //            rec.ID = item.DutyId;
        //            rec.SomeImportantKeyID = 0;
        //            rec.StartDateString = item.StartTime; // "s" is a preset format that outputs as: "2009-02-27T12:12:22"
        //            rec.EndDateString = item.EndTime; // field AppointmentLength is in minutes
        //            rec.Title = item.Location;
        //            rec.StatusString = "";
        //            rec.StatusColor = "#01DF3A:Duty";
        //            string ColorCode = rec.StatusColor.Substring(0, rec.StatusColor.IndexOf(":"));
        //            rec.ClassName = rec.StatusColor.Substring(rec.StatusColor.IndexOf(":") + 1, rec.StatusColor.Length - ColorCode.Length - 1);
        //            rec.StatusColor = ColorCode;
        //            result.Add(rec);
        //        }

        //        return result;
        //    }

        //}


        //public static List<DiaryEvent> LoadAppointmentSummaryInDateRange(double start, double end)
        //{

        //    var fromDate = ConvertFromUnixTimestamp(start);
        //    var toDate = ConvertFromUnixTimestamp(end);
        //    //DateTime cur = DateTime.Now;
        //    using (JCOEntities ent = new JCOEntities())
        //    {
        //        //var rslt = ent.Duties.Where(s => s.DutyDate >= fromDate && s.DutyDate <= toDate);
        //        var rslt = ent.Duties.Where(s => s.DutyDate >= fromDate && s.DutyDate <= toDate);

        //        List<DiaryEvent> result = new List<DiaryEvent>();
        //        int i = 0;
        //        foreach (var item in rslt)
        //        {
        //            DiaryEvent rec = new DiaryEvent();
        //            rec.ID = i; //we dont link this back to anything as its a group summary but the fullcalendar needs unique IDs for each event item (unless its a repeating event)
        //            rec.SomeImportantKeyID = -1;
        //            string StringDate = string.Format("{0:yyyy-MM-dd}", item.DutyDate);
        //            rec.StartDateString = StringDate + "T00:00:00"; //ISO 8601 format
        //            rec.EndDateString = StringDate + "T23:59:59";
        //            rec.Title = "Agent :" + item.TblUser.Name + ", Start Time :" + item.StartTime + ", Location :" + item.Location;
        //            result.Add(rec);
        //            i++;
        //        }

        //        return result;
        //    }

        //}


        //before comment


        //public static void UpdateDiaryEvent(int id, string NewEventStart, string NewEventEnd) 
        //{
        //    // EventStart comes ISO 8601 format, eg:  "2000-01-10T10:00:00Z" - need to convert to DateTime
        //    using (DiaryContainer ent = new DiaryContainer()) {
        //        var rec = ent.AppointmentDiary.FirstOrDefault(s => s.ID == id);
        //        if (rec != null)
        //        {
        //            DateTime DateTimeStart = DateTime.Parse(NewEventStart, null, DateTimeStyles.RoundtripKind).ToLocalTime(); // and convert offset to localtime
        //            rec.DateTimeScheduled = DateTimeStart;
        //            if (!String.IsNullOrEmpty(NewEventEnd)) { 
        //                TimeSpan span = DateTime.Parse(NewEventEnd, null, DateTimeStyles.RoundtripKind).ToLocalTime() - DateTimeStart;
        //                rec.AppointmentLength = Convert.ToInt32(span.TotalMinutes);
        //                }
        //            ent.SaveChanges();
        //        }
        //    }

        //}


        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        //public static bool CreateNewEvent(string Title, string NewEventDate, string NewEventTime, string NewEventDuration)
        //{
        //    try
        //    {
        //        DiaryContainer ent = new DiaryContainer();
        //        AppointmentDiary rec = new AppointmentDiary();
        //        rec.Title = Title;
        //        rec.DateTimeScheduled = DateTime.ParseExact(NewEventDate + " " + NewEventTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        //        rec.AppointmentLength = Int32.Parse(NewEventDuration);
        //        ent.AppointmentDiary.Add(rec);
        //        ent.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}
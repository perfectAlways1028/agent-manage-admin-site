using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtility;
using JcoBAL;
using System.Web.Mvc;
using System.Xml.Linq;
using JcoDAL;

namespace Jco.Filters
{
   static public class CustomMethods
    {

       internal static void BindRoles<T>(T model)
       {
           try
           {
               var roleslist = new RolesBAL { }.GetAllRoles();
               if (roleslist != null)
               {
                   model.GetType().GetProperty("Roles").SetValue(model, roleslist.Select(x => new SelectListItem { Value = x.RoleId.ToString(), Text = x.RoleName }));
               }
           }
           catch (Exception)
           {

               throw;
           }
       }


        internal static void RoleManagement(int pageid)
        {
            try
            {
                int roleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"]);
                var rm = new RolesBAL { }.GetRoleManagement(pageid, roleid);
                if (rm != null)
                {
                    HttpContext.Current.Session["Add"] = rm.Add;
                    HttpContext.Current.Session["Edit"] = rm.Edit;
                    HttpContext.Current.Session["Delete"] = rm.Delete;
                    HttpContext.Current.Session["View"] = rm.View;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static int GetPageIDByPageName(string pagename)
        {
            try
            {
                string str = HttpContext.Current.Server.MapPath("~/RoleManagement.xml");
                var xml = XDocument.Load(str);
                var id = xml.Root.Elements("Page").Where(x => (string)x.Attribute("Name") == pagename)
                                .Select(x => x.Attribute("Id").Value).SingleOrDefault();
                return Convert.ToInt32(id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static bool ValidateRoles(string pagename)
        {
            try
            {
                RoleManagement(GetPageIDByPageName(pagename));
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


    }
}
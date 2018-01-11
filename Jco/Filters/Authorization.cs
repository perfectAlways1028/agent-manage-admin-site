using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jco.Filters
{
    public class Authorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (!skipAuthorization)
            {
                var area = filterContext.RequestContext.RouteData.DataTokens["area"];
                var url = filterContext.HttpContext.Request.Url.AbsoluteUri.ToString();

                if (area != null)
                {
                    var sessionvariable = Convert.ToInt32(HttpContext.Current.Session["AdminUserId"]);
                    if (sessionvariable == 0)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new { Controller = "Logon", Action = "Index", area = "Admin", @returnurl = url }
                            ));
                    }
                }
                else
                {

                }
            }
        }
    }
}
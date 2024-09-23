using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBQA.Filter
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var accountLogin = filterContext.HttpContext.Session["AccountLogin"];

            if (accountLogin == null || (int)accountLogin != 1)
            {
               
                var loginUrl = new UrlHelper(filterContext.RequestContext).Action("Login", "Accounts", new { area = "" });
                filterContext.Result = new RedirectResult(loginUrl);
            }

            base.OnActionExecuting(filterContext);
        }
    }

}
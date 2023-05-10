using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace YRPortal.Controllers
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    // User is authenticated but does not have the required roles
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                else
                {
                    // User is not authenticated
                    var returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery;
                    filterContext.Result = new RedirectResult("AlertUnauthorized");

                    // Store an alert message in the TempData dictionary
                    filterContext.Controller.TempData["Message"] = "You must be logged in to access this page.";
                }
            }
        }
    

    
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YRPortal.Models;

namespace YRPortal.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Membership model)
        {
            using (var context = new PortalEntities3())
            {
                bool isValid = context.Logins.Any(x=> x.Username == model.Username && x.Password == model.Password);
                if (isValid)
                {
                   
                    bool isAdmin = context.Logins.Any(x => x.Role == "Admin");
                    if (isAdmin)
                    {
                        return RedirectToAction("Index", "Logins");
                    }
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                }
                ModelState.AddModelError("", "Invalid username and password");

                
            }
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(Login model)
        {
            using(var context = new PortalEntities3())
            {
                bool isFound = context.Logins.Any(x => x.Username == model.Username);
                if (isFound)
                {
                    ViewBag.ErrorMessage = "Username already found";
                    return RedirectToAction("Signup");
                }
                else if(!isFound && ModelState.IsValid && model.Role == "Student" || model.Role =="Instructor")
                {
                    context.Logins.Add(model);
                    context.SaveChanges();
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                }
            }

            if (model.Role == "Student")
            {
                return RedirectToAction("Create", "Students");
            }
            else if(model.Role == "Instructor")
            {
                return RedirectToAction("Create", "Instructors");
            }
            return RedirectToAction("login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}
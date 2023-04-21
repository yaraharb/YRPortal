using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YRPortal.Config;
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
        public ActionResult Login(Models.Membership model, Login model1)
        {
            using (var context = new PortalEntities4())
            {
                bool isValid = context.Logins.Any(x=> x.Username == model.Username && x.Password == model.Password);
                if (isValid)
                {
                    PortalEntities4 db = new PortalEntities4();
                    String role;
                    using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT Role FROM Login WHERE Username ='" + model.Username + "' AND Password ='" + model.Password + "' ", con))
                        {
                            if (con.State != System.Data.ConnectionState.Open)
                                con.Open();
                            
                            role = (string)cmd.ExecuteScalar();
                        }

                    }
                    if (role.ToString() == "Student")
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                        {
                            using (SqlCommand cmd1 = new SqlCommand("SELECT ID FROM Login WHERE Username ='" + model.Username + "' AND Password ='" + model.Password + "' ", con))
                            {
                                if (con.State != System.Data.ConnectionState.Open)
                                    con.Open();

                                GlobalID.ID = (int)cmd1.ExecuteScalar();
                            }
                        }
                        return RedirectToAction("StudentView", "Courses");
                    }

                     if (role.ToString() == "admin")
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        return RedirectToAction("Index", "Logins");
                    }

                    if (role.ToString() == "Instructor")
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        return RedirectToAction("Create", "Instructors");
                    }
                }
                else { 
                ModelState.AddModelError("", "Invalid username and password");
                }
                
            }
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(Login model)
        {
            using(var context = new PortalEntities4())
            {
                bool isFound = context.Logins.Any(x => x.Username == model.Username);
                if (isFound)
                {
                    ViewBag.ErrorMessage = "Username already found";
                    return RedirectToAction("Signup");
                }
                else if(!isFound && ModelState.IsValid && model.Role == "Student" || model.Role =="Instructor")
                {   
                    if(model.Role == "Student")
                    {
                        model.Username = model.Username + "@bb.edu";
                    }
                    if (model.Role == "Instructor")
                    {
                        model.Username = model.Username + "@bb.edu.lb";
                    }
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

using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YRPortal.Config;
using YRPortal.Models;

namespace YRPortal.Controllers
{
    public class LoginsController : Controller
    {
        private PortalEntities4 db = new PortalEntities4();

        // GET: Logins
        public ActionResult Index()
        {

            return View(db.Logins.ToList());
        }

        // GET: Logins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Logins.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // GET: Logins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Logins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,firstname,lastname,description, role")] CreateUser model)
        {
            int idd = 1;
            using (var context = new PortalEntities4())
            {
                Login user = new Login();
                string username = model.firstname + "." + model.lastname;
                user.Username = username;
                user.Password = username;
                user.Role = model.role;

                bool isFound = context.Logins.Any(x => x.Username == username);
                if (isFound)
                {
                    ViewBag.ErrorMessage = "Username already found";
                    return RedirectToAction("");
                }
                else if (!isFound && ModelState.IsValid && (model.role == "Student" || model.role == "Instructor"))
                {
                    db.Logins.Add(user);
                    db.SaveChanges();
                    using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT ID FROM Login WHERE username = '" + username + "' ; ", con))
                        {
                            if (con.State != System.Data.ConnectionState.Open)
                                con.Open();
                            SqlDataReader sdr = cmd.ExecuteReader();
                            DataTable dt = new DataTable();
                            dt.Load(sdr);
                            foreach (DataRow row in dt.Rows)
                                idd = int.Parse(row["ID"].ToString());
                        }
                    }

                    
                    if (model.role == "Student")
                    {
                        Student student = new Student();
                        student.FName = model.firstname;
                        student.Lname = model.lastname;
                        student.Major = model.description;
                        student.LoginId = idd;

                        db.Students.Add(student);
                    }
                    if (model.role == "Instructor")
                    {
                        Instructor instructor = new Instructor();
                        instructor.Fname = model.firstname;
                        instructor.lname = model.lastname;
                        instructor.Salary = int.Parse(model.description);
                        instructor.LoginID = idd;

                        db.Instructors.Add(instructor);
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(model);
            }
        }





        // GET: Logins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Logins.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // POST: Logins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Username,Password,Role")] Login login)
        {
            db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            
        }

        // GET: Logins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Logins.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // POST: Logins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var note = db.Notes.Where(n => n.author == id);
            if (note != null)
            {
                db.Notes.RemoveRange(note);
            }

            var task = db.Tasks.Where(n => n.StudentID == id);
            if (task != null)
            {
                db.Tasks.RemoveRange(task);
            }
            var e = db.EnrollsIns.Where(n => n.LoginID == id);
            if (e != null)
            {
                db.EnrollsIns.RemoveRange(e);
            }


            var t = db.Teaches.Where(n => n.LoginID == id);
            if (t != null)
            {
                db.Teaches.RemoveRange(t);
            }
            var student = db.Students.Where(n => n.LoginId == id);
            if (student != null)
            {
                db.Students.RemoveRange(student);
            }


            var instructor = db.Instructors.Where(n => n.LoginID == id);
            if (instructor != null)
            {
                db.Instructors.RemoveRange(instructor);
            }

            var role = db.UserRoles.Where(n => n.UserId == id);
            if(role != null) 
            {
                db.UserRoles.RemoveRange(role);
            }

            

            Login login = db.Logins.Find(id);
            db.Logins.Remove(login);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

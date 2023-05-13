using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using YRPortal.Config;
using YRPortal.Models;

namespace YRPortal.Controllers
{
    [Authorize]
    public class InstructorsController : Controller
    {
        private PortalEntities4 db = new PortalEntities4();
        
        // GET: Instructors
        public ActionResult Index()
        {
            return View(db.Instructors.ToList());
        }
        public ActionResult changePassword()
        {
            return View();
        }
        // GET: Instructors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InstructorID,Fname,Lname")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstructorID,Fname,Lname")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(instructor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(instructor);
        }

        // GET: Instructors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = db.Instructors.Find(id);
            db.Instructors.Remove(instructor);
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
        public ActionResult AlertUnauthorized()
        {
            // Get the alert message from TempData
            string alertMessage = TempData["AlertMessage1"] as string;

            // Pass the alert message to the view
            ViewBag.AlertMessage = alertMessage;

            return View();
        }
        static int InstructorID;
        public ActionResult Assign(int id)
        {
            
            List<int> AssignedCourses = new List<int>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT CourseID FROM Teaches ", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        AssignedCourses.Add((int)row["CourseID"]);
                    }


                }
            }
            if (AssignedCourses.Contains(id))
            {
                TempData["AlertMessage1"] = "You are already enrolled in this course!";
                return RedirectToAction("AlertUnauthorized");
            }
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Teaches(LoginID, CourseID) VALUES(@LoginID, @CourseID)", con))
                {
                    cmd.Parameters.AddWithValue("@LoginID", InstructorID) ;
                    cmd.Parameters.AddWithValue("@CourseID", id);
                    cmd.ExecuteNonQuery();
                }

            }
            return RedirectToAction("Index");
            
        }

        public ActionResult AssignList(int id)
        {
            List<int> ids = new List<int>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(" SELECT i.LoginID FROM Instructor i Where i.InstructorID = '" + id + "' ", con))
                {

                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        ids.Add((int)row["LoginID"]);
                        InstructorID = (int)row["LoginID"];
                    }

                }
            }
            //InstructorID = (int)instructor.LoginID;
            List<int> AssignedCourses = new List<int>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT CourseID FROM Teaches ", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        AssignedCourses.Add((int)row["CourseID"]);
                    }


                }
            }
            List<Course> Courses = db.Courses.ToList();
            foreach (int courseId in AssignedCourses)
            {
                Courses.RemoveAll(c => c.CourseID == courseId);
            }
            return View(Courses);
        }
    }
}

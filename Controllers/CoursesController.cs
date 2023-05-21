using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Pkcs;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;
using System.Xml.Linq;
using YRPortal.Config;
using YRPortal.Models;

namespace YRPortal.Controllers
{
    public class CoursesController : Controller
    {
        private PortalEntities4 db = new PortalEntities4();

        // GET: Courses

        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }
 
        public ActionResult StudentView()
        {

            List<Course> courses = new List<Course>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Course, EnrollsIn WHERE EnrollsIn.LoginID ='" + GlobalID.ID + "' AND Course.CourseID = EnrollsIn.CourseID ", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        courses.Add(
                            new Course
                            {
                                CourseID = (int)row["CourseID"],
                                Name = row["Name"].ToString(),
                                Description = row["Description"].ToString()
                            });;
                    }


                }
            }
            return View(courses);
        }
        public ActionResult InstructorView()
        {

            List<Course> courses = new List<Course>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Course, Teaches WHERE Teaches.LoginID ='" + GlobalID.ID + "' AND Course.CourseID = Teaches.CourseID ", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        courses.Add(
                            new Course
                            {
                                Name = row["Name"].ToString(),
                                Description = row["Description"].ToString()
                            });
                    }


                }
            }
            return View(courses);
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "CourseID,Name,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        public ActionResult EnrollList()
        {
            List<int> CoursesTaken = new List<int>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Course, EnrollsIn WHERE EnrollsIn.LoginID ='" + GlobalID.ID + "' AND Course.CourseID = EnrollsIn.CourseID ", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        CoursesTaken.Add((int)row["CourseID"]);
                    }


                }
            }
            List<Course> Courses = db.Courses.ToList();
            foreach (int courseId in CoursesTaken)
            {
                Courses.RemoveAll(c => c.CourseID == courseId);
            }
            return View(Courses);
        }


        public ActionResult EnrollIn(int id)
        {
            List<int> RegisteredCourses = new List<int>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT CourseID FROM EnrollsIn WHERE EnrollsIn.LoginID ='" + GlobalID.ID + "' ", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        RegisteredCourses.Add((int)row["CourseID"]);
                    }


                }
            }
            if (RegisteredCourses.Contains(id))
            {
                TempData["AlertMessage"] = "You are already enrolled in this course!";
                return RedirectToAction("Alert");
            }
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO EnrollsIn(LoginID, CourseID) VALUES(@LoginID, @CourseID)", con))
                    {
                        cmd.Parameters.AddWithValue("@LoginID", GlobalID.ID);
                        cmd.Parameters.AddWithValue("@CourseID", id);
                        cmd.ExecuteNonQuery();
                    }
                
            }
            return RedirectToAction("StudentView");
        }

        public ActionResult Alert()
        {
            // Get the alert message from TempData
            string alertMessage = TempData["AlertMessage"] as string;

            // Pass the alert message to the view
            ViewBag.AlertMessage = alertMessage;

            return View();
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseID,Name,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        public ActionResult Delete(int? id)
        {
            if (id  == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
    
        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);


            var notes = db.Notes.Where(n => n.courseName == id.ToString());
            if (notes != null)
            {
                db.Notes.RemoveRange(notes);
                db.SaveChanges();
            }
            var reviews = db.Reviews.Where(r => r.CourseID == id);
            if (reviews != null)
            {
                db.Reviews.RemoveRange(reviews);
                db.SaveChanges();
            }

            var enrolls = db.EnrollsIns.Where(e => e.CourseID == id);
            if (enrolls != null)
            {
                db.EnrollsIns.RemoveRange(enrolls);
                db.SaveChanges();
            }
            var teaches = db.Teaches.Where(t => t.CourseID == id);
            if (teaches != null)
            {
                db.Teaches.RemoveRange(teaches);
                db.SaveChanges();
            }

            var materials = db.materials.Where(m => m.courseID == id);
            if (materials != null)
            {
                db.materials.RemoveRange(materials);
                db.SaveChanges();
            }
            db.SaveChanges();
            db.Courses.Remove(course);
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
        public ActionResult material(int id)
        {

            List<material> materials = new List<material>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT m.ID, m.title, m.content, c.Name FROM Material m INNER JOIN Course c ON m.courseID = c.CourseID INNER JOIN EnrollsIn t ON c.CourseID = t.CourseID WHERE t.LoginID = '" + GlobalID.ID + "'", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        materials.Add(
                            new material
                            {
                                ID = (int)row["ID"],
                                title = row["title"].ToString(),
                                content = row["content"].ToString(),
                                courseName = row["Name"].ToString()
                            });
                    }


                }
            }
            return View(materials);
        }

        public ActionResult Drop(int id) {
            var enrolls = db.EnrollsIns.Where(e => e.CourseID == id);
            if (enrolls != null)
            {
                db.EnrollsIns.RemoveRange(enrolls);
            }
            db.SaveChanges();
            return RedirectToAction("../Courses/StudentView");

        }
        public ActionResult DisplayFile(string filePath)
        {
            // Create a file stream from the file path
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            // Extract the file name from the file path
            string fileName = Path.GetFileName(filePath);

            // Return the file as a FileStreamResult, providing appropriate content type
            return File(fileStream, "application/octet-stream", fileName);
        }
    
    }
}

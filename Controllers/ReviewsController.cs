using Microsoft.Ajax.Utilities;
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
    public class ReviewsController : Controller
    {
        private PortalEntities4 db = new PortalEntities4();

        // GET: Reviews
        public ActionResult Index()
        {
            List<Review> reviews = new List<Review>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Review WHERE StudentID = '" + GlobalID.ID + "'", con))

                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        int CourseID = (int)row["CourseID"];
                        Course c = db.Courses.Find(CourseID);
                        string CN = c.Name;

                        reviews.Add(
                            new Review
                            {
                                ID = int.Parse(row["ID"].ToString()),
                                Explanation = int.Parse(row["Explanation"].ToString()),
                                Content = int.Parse(row["Content"].ToString()),
                                Relevance = int.Parse(row["Relevance"].ToString()),
                                Comment = row["Comment"].ToString(),
                                CourseID = (int?)row["CourseID"],
                                CourseName = c.Name,
                            });
                    }


                }
            }

            return View(reviews);
        }
        public ActionResult Admin()
        {
            List<Review> reviews = new List<Review>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Review", con))

                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        int CourseID = (int)row["CourseID"];
                        Course c = db.Courses.Find(CourseID);
                        string CN = c.Name;

                        reviews.Add(
                            new Review
                            {
                                ID = int.Parse(row["ID"].ToString()),
                                Explanation = int.Parse(row["Explanation"].ToString()),
                                Content = int.Parse(row["Content"].ToString()),
                                Relevance = int.Parse(row["Relevance"].ToString()),
                                Comment = row["Comment"].ToString(),
                                CourseID = (int?)row["CourseID"],
                                CourseName = c.Name,
                            });
                    }


                }
            }

            return View(reviews);
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            List<String> names = new List<String>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(" SELECT c.Name  FROM Course c INNER JOIN EnrollsIn e ON c.CourseID = e.CourseID INNER JOIN Login s ON e.LoginID = s.ID WHERE s.ID = '" + GlobalID.ID + "' ", con))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        names.Add(row["Name"].ToString());
                    }


                }
            }

            ViewBag.CourseName = new SelectList(names);
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CourseID,StudentID,Explanation,Content,Relevance,Comment, CourseName")] Review review)
        {

            //Get the list of the already submitted reviews from the same student
            List<Review> reviews = new List<Review>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Review WHERE StudentID = '" + GlobalID.ID + "'", con))

                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        reviews.Add(
                            new Review
                            {
                                ID = int.Parse(row["ID"].ToString()),
                                CourseID = int.Parse(row["CourseID"].ToString()),
                                Explanation = int.Parse(row["Explanation"].ToString()),
                                Content = int.Parse(row["Content"].ToString()),
                                Relevance = int.Parse(row["Relevance"].ToString()),
                                Comment = row["Comment"].ToString(),
                            });
                    }


                }
            }

            //Get the id of the course that the student wants to review right now
            List<int> ids = new List<int>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(" SELECT c.CourseID FROM Course c Where c.Name = '" + review.CourseName + "' ", con))
                {

                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        ids.Add((int)row["CourseID"]);
                        review.CourseID = (int)row["CourseID"];
                    }

                }
            }
            Boolean found = false;
            foreach (Review review1 in reviews)
            {
                if (review1.CourseID == review.CourseID)
                {
                    ViewBag.error = "You have already submitted a review for this course";
                    found = true;
                    return RedirectToAction("../Reviews/Index");
                }
            }

           

            if ( !found)
            {
                review.StudentID = GlobalID.ID;
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Name", review.CourseID);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Name", review.CourseID);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CourseID,StudentID,Explanation,Content,Relevance,Comment")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                review.StudentID = GlobalID.ID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Name", review.CourseID);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
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

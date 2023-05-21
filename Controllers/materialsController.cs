using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using YRPortal.Config;
using YRPortal.Models;

namespace YRPortal.Controllers
{
    public class materialsController : Controller
    {
        private PortalEntities4 db = new PortalEntities4();

        // GET: materials
        public ActionResult Index()
        {
            List<material> materials = new List<material>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT m.ID, m.title, m.content, c.Name FROM Material m INNER JOIN Course c ON m.courseID = c.CourseID INNER JOIN Teaches t ON c.CourseID = t.CourseID WHERE t.LoginID = '" + GlobalID.ID + "'", con))


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


        public ActionResult DisplayFile(string filePath)
        {
            // Create a file stream from the file path
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            // Extract the file name from the file path
            string fileName = Path.GetFileName(filePath);

            // Return the file as a FileStreamResult, providing appropriate content type
            return File(fileStream, "application/octet-stream", fileName);
        }
        // GET: materials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            material material = db.materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }
        // GET: materials/Create
        public ActionResult Create()
        {
            List<String> names = new List<String>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(" SELECT c.Name FROM Course c INNER JOIN Teaches e ON c.CourseID = e.CourseID INNER JOIN Login s ON e.LoginID = s.ID WHERE s.ID = '" + GlobalID.ID + "' ", con))
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

            ViewBag.courseName = new SelectList(names);
            return View();
        }

        // POST: materials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( material material)
        {
            if (ModelState.IsValid)
            {
                string extension = Path.GetExtension(material.UploadMaterial.FileName);
                string fileName = DateTime.Now.ToString("yymmssfff") + material.UploadMaterial.FileName;
                string imageName = fileName;

                material.content = "~/materials/" + fileName;

                fileName = Path.Combine(Server.MapPath("~/materials/"), fileName);
                material.content = fileName;
                material.UploadMaterial.SaveAs(fileName);
                List<int> ids = new List<int>();
                using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                {
                    using (SqlCommand cmd = new SqlCommand(" SELECT c.CourseID FROM Course c Where c.Name = '" +material.courseName +"' ", con))
                    {
                        
                        if (con.State != System.Data.ConnectionState.Open)
                            con.Open();
                        SqlDataReader sdr = cmd.ExecuteReader();

                        DataTable dt = new DataTable();
                        dt.Load(sdr);
                        foreach (DataRow row in dt.Rows)
                        {
                            ids.Add((int)row["CourseID"]);
                            material.courseID = (int)row["CourseID"];
                        }

                    }
                }

                material.courseID = ids[0];

                db.materials.Add(material);
                db.SaveChanges();
                return RedirectToAction("../materials/Index");
            }
            
            return View(material);
        }

        // GET: materials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            material material = db.materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            ViewBag.courseID = new SelectList(db.Courses, "CourseID", "Name", material.courseID);
            return View(material);
        }

        // POST: materials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,title,content,courseID")] material material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.courseID = new SelectList(db.Courses, "CourseID", "Name", material.courseID);
            return View(material);
        }

        // GET: materials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            material material = db.materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        // POST: materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            material material = db.materials.Find(id);
            db.materials.Remove(material);
            db.SaveChanges();
            return RedirectToAction("../materials/Index");
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

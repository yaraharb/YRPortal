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
    public class NotesController : Controller
    {
        private PortalEntities4 db = new PortalEntities4();

        // GET: Notes
        public ActionResult Index()
        {
            List<Note> notes = new List<Note>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Note WHERE author = '" + GlobalID.ID + "' or courseName IN( SELECT c.Name FROM Course c INNER JOIN EnrollsIn e ON c.CourseID = e.CourseID INNER JOIN Login s ON e.LoginID = s.ID WHERE s.ID = '" + GlobalID.ID + "' and status = 'public'); ", con))

                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    foreach (DataRow row in dt.Rows)
                    {
                        notes.Add(
                            new Note
                            {
                                id = int.Parse(row["id"].ToString()),
                                title = row["title"].ToString(),
                                content = row["content"].ToString(),
                                courseName = row["courseName"].ToString(),
                                author = int.Parse(row["author"].ToString()),
                                status = row["status"].ToString(),
                            }) ;
                    }


                }
            }
            
            return View(notes);
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

        // GET: Notes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // GET: Notes/Create
        public ActionResult Create()
        {




            List<String> names = new List<String>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(" SELECT c.Name FROM Course c INNER JOIN EnrollsIn e ON c.CourseID = e.CourseID INNER JOIN Login s ON e.LoginID = s.ID WHERE s.ID = '" + GlobalID.ID + "' ", con))
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
            ViewBag.author = new SelectList(db.Logins, "ID", "Username");
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,UploadFile,status,author,courseName")] Note note)
        {
            if (ModelState.IsValid)
            {
                string extension = Path.GetExtension(note.UploadFile.FileName);
                string fileName = DateTime.Now.ToString("yymmssfff")  + note.UploadFile.FileName;
                string imageName = fileName;

                note.content = "~/notes/" + fileName;

                fileName = Path.Combine(Server.MapPath("~/notes/"), fileName);
                note.content = fileName;
                note.UploadFile.SaveAs(fileName);
                note.author = GlobalID.ID;



            



                db.Notes.Add(note);
                db.SaveChanges();
                return RedirectToAction("../Notes/Index");
            }
            ViewBag.courseName = new SelectList(db.Logins, "ID", "Username", note.courseName);
            ViewBag.author = new SelectList(db.Logins, "ID", "Username", note.author);
            return View(note);
        }

        // GET: Notes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.author = new SelectList(db.Logins, "ID", "Username", note.author);
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,content,status,author,courseName")] Note note)
        {
            if (ModelState.IsValid)
            {
                db.Entry(note).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Notes/Index");
            }
            ViewBag.author = new SelectList(db.Logins, "ID", "Username", note.author);
            return View(note);
        }

        // GET: Notes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = db.Notes.Find(id);
            db.Notes.Remove(note);
            db.SaveChanges();
            return RedirectToAction("../Notes/Index");
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

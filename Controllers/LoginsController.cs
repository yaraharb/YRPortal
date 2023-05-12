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
        public async Task<ActionResult> Edit(int? id)
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.usbeefme.com.192-185-6-199.hgws3.hgwin.temp.domains/api/Recipes/GetRecipes");
            request.Headers.Add("Authorization", "Bearer TBoLIS0frPn51cLUleenUnkldKIBpQVRvgWs6BZjbQeTQbpjxvhZqkkJQm4hpfytNg_gnvhCe7nPDoSU7vLItA6EyMzIb1D0deJuGLTfR0QeIfqW-LrB_dKc6tLqhvPLPvti_buJVn68tu1foONznVvlbSk4rR09IlE1l-LDS5w6YK19sofDcHuGvz-GWFtxiIKxb84s8lFishISwJt3gG5SRJ1EGuBsvqsnU9U5EDG-TGKKzhLUQGwR4TX8i5sCheibep4AmaHzvlTKwPADJMnZbTUjvE9643YBgyub4XG0YImsVdTajZ0ihQCtzyuWDi-5cmFp4P1uXTEhqeS6TJkUdBYrhAqCPImSGVd-yuZJE06QylrJL36f-hKgb9WDOhxVaFqgjLUbh7Y3yNdZsDzvbTQDgaf2LZSYdzozfqSxDbGjNDWY007iRSZWEbW4Jjfx9odGt-o62Qhg3LcuLPNqWYaf0uMSYZRcv7NS29bhubAvWZfuxahVHEco5Zo8NTX9IA");
            var content = new StringContent("{\r\n  \"offset\": 1,\r\n  \"pageSize\": 10,\r\n  \"categoryID\": \"3\",\r\n}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return View();
        }

        // POST: Logins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Username,Password,Role")] Login login)
        {
            if (ModelState.IsValid)
            {
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(login);
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

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Obuchalkin.Models;
using BCrypt.Net;

namespace Obuchalkin.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private ObuchalkinEntities db = new ObuchalkinEntities();

        // GET: Users/Index
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Login,Password,FullName,Position")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Хешируем пароль
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Login,Password,FullName,Position")] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Find(user.Login);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Position = user.Position;
                    if (!string.IsNullOrEmpty(user.Password) && !user.Password.StartsWith("$2a$") && !user.Password.StartsWith("$2b$"))
                    {
                        existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    }
                    db.Entry(existingUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Obuchalkin.Models;

namespace Obuchalkin.Controllers
{
    [Authorize]
    public class ResultsController : Controller
    {
        private ObuchalkinEntities db = new ObuchalkinEntities();

        // GET: Results/Index
        public ActionResult Index()
        {
            var results = db.Results.Include(r => r.User);
            return View(results.ToList());
        }

        // GET: Results/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // GET: Results/Create
        public ActionResult Create()
        {
            ViewBag.Login = new SelectList(db.Users, "Login", "FullName");
            return View();
        }

        // POST: Results/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Login,Course,AttemptsUsed,BestResult")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Results.Add(result);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Login = new SelectList(db.Users, "Login", "FullName", result.Login);
            return View(result);
        }

        // GET: Results/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            ViewBag.Login = new SelectList(db.Users, "Login", "FullName", result.Login);
            return View(result);
        }

        // POST: Results/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Login,Course,AttemptsUsed,BestResult")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Login = new SelectList(db.Users, "Login", "FullName", result.Login);
            return View(result);
        }

        // GET: Results/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Result result = db.Results.Find(id);
            db.Results.Remove(result);
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
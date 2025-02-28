using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Obuchalkin.Models;

namespace Obuchalkin.Controllers
{
    public class ProgressesController : Controller
    {
        private ObuchalkinEntities db = new ObuchalkinEntities();

        // GET: Progresses
        public ActionResult Index()
        {
            var progresses = db.Progresses.Include(p => p.User);
            return View(progresses.ToList());
        }

        // GET: Progresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Progress progress = db.Progresses.Find(id);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return View(progress);
        }

        // GET: Progresses/Create
        public ActionResult Create()
        {
            ViewBag.Login = new SelectList(db.Users, "Login", "Password");
            return View();
        }

        // POST: Progresses/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Login,Course,Section,Page,TestCompleted")] Progress progress)
        {
            if (ModelState.IsValid)
            {
                db.Progresses.Add(progress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Login = new SelectList(db.Users, "Login", "Password", progress.Login);
            return View(progress);
        }

        // GET: Progresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Progress progress = db.Progresses.Find(id);
            if (progress == null)
            {
                return HttpNotFound();
            }
            ViewBag.Login = new SelectList(db.Users, "Login", "Password", progress.Login);
            return View(progress);
        }

        // POST: Progresses/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Login,Course,Section,Page,TestCompleted")] Progress progress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(progress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Login = new SelectList(db.Users, "Login", "Password", progress.Login);
            return View(progress);
        }

        // GET: Progresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Progress progress = db.Progresses.Find(id);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return View(progress);
        }

        // POST: Progresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Progress progress = db.Progresses.Find(id);
            db.Progresses.Remove(progress);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Obuchalkin.Models;

namespace Obuchalkin.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private ObuchalkinEntities db = new ObuchalkinEntities();

        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Обучающийся")
            {
                return RedirectToAction("Index", "Authorization");
            }
            return View();
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
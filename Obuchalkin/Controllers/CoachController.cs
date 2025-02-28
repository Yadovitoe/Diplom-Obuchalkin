using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Obuchalkin.Controllers
{
    public class CoachController : Controller
    {
        // GET: Couch
        public ActionResult Index()
        {
            return View();
        }

        // GET: Couch/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Couch/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Couch/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Couch/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Couch/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Couch/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Couch/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

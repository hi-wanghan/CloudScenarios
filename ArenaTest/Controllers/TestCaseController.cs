using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArenaTest.Models;

namespace ArenaTest.Controllers
{
    public class TestCaseController : Controller
    {
        private ArenaTestModelsContainer db = new ArenaTestModelsContainer();

        // GET: /TestCase/
        public ActionResult Index()
        {
            return View(db.TestCases.ToList());
        }

        // GET: /TestCase/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCase testcase = db.TestCases.Find(id);
            if (testcase == null)
            {
                return HttpNotFound();
            }
            return View(testcase);
        }

        // GET: /TestCase/Create
        public ActionResult Create(int testId)
        {
            TestCase tc = new TestCase { TestId = testId };
            return View(tc);
        }

        // POST: /TestCase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TestCaseId,Metrics,Method,TestId")] TestCase testcase)
        {
            if (ModelState.IsValid)
            {
                
                db.TestCases.Add(testcase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testcase);
        }

        // GET: /TestCase/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCase testcase = db.TestCases.Find(id);
            if (testcase == null)
            {
                return HttpNotFound();
            }
            return View(testcase);
        }

        // POST: /TestCase/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="TestCaseId,Metrics,Method,TestId")] TestCase testcase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testcase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testcase);
        }

        // GET: /TestCase/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCase testcase = db.TestCases.Find(id);
            if (testcase == null)
            {
                return HttpNotFound();
            }
            return View(testcase);
        }

        // POST: /TestCase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestCase testcase = db.TestCases.Find(id);
            db.TestCases.Remove(testcase);
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

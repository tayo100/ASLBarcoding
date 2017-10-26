using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASLBarcoding.Models;

namespace ASLBarcoding.Controllers
{
    public class TestCheckListController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();

        // GET: TestCheckList
        public ActionResult Index()
        {
            var TestCheckList = db.TestCheckList.Include(t => t.Request);
            return View(TestCheckList.ToList());
        }

        // GET: TestCheckList/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCheckList testCheckList = db.TestCheckList.Find(id);
            if (testCheckList == null)
            {
                return HttpNotFound();
            }
            return View(testCheckList);
        }

        // GET: TestCheckList/Create
        public ActionResult Create()
        {
            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID");
            return View();
        }

        // POST: TestCheckList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RequestId")] TestCheckList testCheckList)
        {
            if (ModelState.IsValid)
            {
                db.TestCheckList.Add(testCheckList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID", testCheckList.RequestId);
            return View(testCheckList);
        }

        // GET: TestCheckList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCheckList testCheckList = db.TestCheckList.Find(id);
            if (testCheckList == null)
            {
                return HttpNotFound();
            }
            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID", testCheckList.RequestId);
            return View(testCheckList);
        }

        // POST: TestCheckList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RequestId")] TestCheckList testCheckList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testCheckList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID", testCheckList.RequestId);
            return View(testCheckList);
        }

        // GET: TestCheckList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCheckList testCheckList = db.TestCheckList.Find(id);
            if (testCheckList == null)
            {
                return HttpNotFound();
            }
            return View(testCheckList);
        }

        // POST: TestCheckList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestCheckList testCheckList = db.TestCheckList.Find(id);
            db.TestCheckList.Remove(testCheckList);
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

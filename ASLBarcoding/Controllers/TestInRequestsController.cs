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
    public class TestInRequestController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();

        // GET: TestInRequest
        public ActionResult Index()
        {
            var testInRequest = db.TestInRequest.Include(t => t.Request).Include(t => t.Test);/*.Include(t => t.TestType);*/
            return View(testInRequest.ToList());
        }

        // GET: TestInRequest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestInRequest testInRequest = db.TestInRequest.Find(id);
            if (testInRequest == null)
            {
                return HttpNotFound();
            }
            return View(testInRequest);
        }

        // GET: TestInRequest/Create
        public ActionResult Create()
        {
            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID");
            ViewBag.TestId = new SelectList(db.Test, "Id", "Name");
            ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name");
            return View();
        }

        // POST: TestInRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RequestId,TestId,TestTypeId,SelectedTest")] TestInRequest testInRequest)
        {
            if (ModelState.IsValid)
            {
                db.TestInRequest.Add(testInRequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID", testInRequest.RequestId);
            ViewBag.TestId = new SelectList(db.Test, "Id", "Name", testInRequest.TestId);
            //ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name", testInRequest.TestTypeId);
            return View(testInRequest);
        }

        // GET: TestInRequest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestInRequest testInRequest = db.TestInRequest.Find(id);
            if (testInRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID", testInRequest.RequestId);
            ViewBag.TestId = new SelectList(db.Test, "Id", "Name", testInRequest.TestId);
           // ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name", testInRequest.TestTypeId);
            return View(testInRequest);
        }

        // POST: TestInRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RequestId,TestId,TestTypeId,SelectedTest")] TestInRequest testInRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testInRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RequestId = new SelectList(db.Request, "ID", "ID", testInRequest.RequestId);
            ViewBag.TestId = new SelectList(db.Test, "Id", "Name", testInRequest.TestId);
           // ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name", testInRequest.TestTypeId);
            return View(testInRequest);
        }

        // GET: TestInRequest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestInRequest testInRequest = db.TestInRequest.Find(id);
            if (testInRequest == null)
            {
                return HttpNotFound();
            }
            return View(testInRequest);
        }

        // POST: TestInRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestInRequest testInRequest = db.TestInRequest.Find(id);
            db.TestInRequest.Remove(testInRequest);
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

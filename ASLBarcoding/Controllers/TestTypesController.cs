using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASLBarcoding.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ASLBarcoding.Controllers
{
    public class TestTypeController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();

        // GET: TestType
        public ActionResult Index()
        {
            return View(db.TestType.ToList());
        }

        // GET: TestType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestType testType = db.TestType.Find(id);
            if (testType == null)
            {
                return HttpNotFound();
            }
            return View(testType);
        }

        // GET: TestType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Name")] TestType testType)
        {
            if (ModelState.IsValid)
            {

                //var manager = new UserManager<AspNetUser>(new UserStore<AspNetUser>(new ApplicationDbContext()));

                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (currentUser != null)
                    testType.createdBy = currentUser.UserName;
                else
                    testType.createdBy = User.Identity.Name;

                db.TestType.Add(testType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testType);
        }

        // GET: TestType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestType testType = db.TestType.Find(id);
            if (testType == null)
            {
                return HttpNotFound();
            }
            return View(testType);
        }

        // POST: TestType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Name")] TestType testType)
        {
            if (ModelState.IsValid)
            {

                var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (currentUser != null)
                    testType.updatedBy = currentUser.UserName;
                else
                    testType.updatedBy = User.Identity.Name;

                testType.updatedDate = DateTime.Now;


                db.Entry(testType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testType);
        }

        // GET: TestType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestType testType = db.TestType.Find(id);
            if (testType == null)
            {
                return HttpNotFound();
            }
            return View(testType);
        }

        // POST: TestType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestType testType = db.TestType.Find(id);
            db.TestType.Remove(testType);
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

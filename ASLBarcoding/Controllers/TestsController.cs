using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASLBarcoding.Models;
using PagedList;
using Microsoft.AspNet.Identity;

namespace ASLBarcoding.Controllers
{
    public class TestsController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();

        // GET: Tests1
        public ActionResult Index(string sortOrder)//, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.TestNameSortParm = String.IsNullOrEmpty(sortOrder) ? "TestName_desc" : "";
            ViewBag.TestTypeSortParm = sortOrder == "TestType" ? "TestType_desc" : "TestType";
            ViewBag.OiCNameSortParm = sortOrder == "OiC" ? "oicname_desc" : "OiC";
            ViewBag.CostSortParm = sortOrder == "Cost" ? "cost_desc" : "Cost";

            var test = from r in db.Test select r;

            switch (sortOrder)
            {
                case "TestName_desc":
                    test = test.OrderByDescending(rg => rg.Name);
                    break;
                case "TestType_desc":
                    test = test.OrderByDescending(rg => rg.TestType.Name);
                    break;
                case "TestType":
                    test = test.OrderBy(rg => rg.TestType.Name);
                    break;
                case "oicname_desc":
                    test = test.OrderByDescending(rg => rg.AspNetUser.LastName).ThenBy(rg => rg.AspNetUser.FirstName);
                    break;
                case "OiC":
                    test = test.OrderBy(rg => rg.AspNetUser.LastName).ThenBy(rg => rg.AspNetUser.FirstName);
                    break;
                case "Cost":
                    test = test.OrderBy(rg => rg.Cost);
                    break;
                case "Cost_desc":
                    test = test.OrderByDescending(rg => rg.Cost);
                    break;
                default:  // Name ascending 
                    test = db.Test.Include(t => t.AspNetUser).Include(t => t.TestType); ;
                    break;
            }

            //if (TempData["msg"] != null && TempData["msg"].ToString().Length > 0) { ViewBag.Message = TempData["msg"].ToString(); Session.Clear(); }

            //int pageSize = 10;
            //int pageNumber = (page ?? 1);

            //return View(test.ToPagedList(pageNumber, pageSize));

            //var test = db.Test.Include(t => t.AspNetUser).Include(t => t.TestType);
            return View(test.ToList());
        }

        // GET: Tests1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // GET: Tests1/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUser, "Id", "FullName");
            ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name");
            return View();
        }

        // POST: Tests1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TestTypeId,UserId,Cost")] Test test)
        {
            if (ModelState.IsValid)
            {

                var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());
                //barcodecs objbar = new barcodecs();

                if (currentUser != null)
                    test.createdBy = currentUser.UserName;
                else
                    test.createdBy = User.Identity.Name;

                test.createdDate = DateTime.Now;


                db.Test.Add(test);
                db.SaveChanges();

                var testchk = new TestCheck();
                testchk.Checked = false;
                testchk.TestId = test.Id;
                db.TestCheck.Add(testchk);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUser, "Id", "FullName");
            ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name", test.TestTypeId);
            return View(test);
        }

        // GET: Tests1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUser, "Id", "FullName");
            ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name", test.TestTypeId);
            return View(test);
        }

        // POST: Tests1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TestTypeId,UserId,Cost")] Test test)
        {
            if (ModelState.IsValid)
            {

                var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (currentUser != null)
                    test.updatedBy = currentUser.UserName;
                else
                    test.updatedBy = User.Identity.Name;

                test.updatedDate = DateTime.Now;

                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUser, "Id", "FullName");
            ViewBag.TestTypeId = new SelectList(db.TestType, "Id", "Name", test.TestTypeId);
            return View(test);
        }

        // GET: Tests1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: Tests1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Test test = db.Test.Find(id);
            db.Test.Remove(test);

            var tchk = db.TestCheck.Where(x => x.TestId == id).FirstOrDefault();
            db.TestCheck.Remove(tchk);

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

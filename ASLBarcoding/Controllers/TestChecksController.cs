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

namespace ASLBarcoding.Controllers
{
    public class TestCheckController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();

        // GET: TestCheck
        [HttpGet]
        public ActionResult Index()
        {
            var ttype = Convert.ToInt32(TempData["ttype"]);
            var TestCheck = db.TestCheck.Where(t => t.Test.TestTypeId == ttype);

            return View(TestCheck.ToList());
        }

        [HttpPost]
        public ActionResult Index(List<TestCheck> list)
        {
            //save this list to table tracking selected items. A row should look like:
            //id  -  listitemtosave(Testelected)(Testid) - forwhichrequestinstance(RequestId) - 
            //            return View(list);

            var testInRequest = new TestInRequest();
            var varId = Convert.ToInt32(TempData["reqId"]);
            decimal? cost = 0.00m;
            //var no = db.Request.Where(x => x.ID == varId).Select(x => x.NoofSamples).FirstOrDefault();


            //var count = 1;
            foreach (TestCheck tchk in list)
            {
                if (tchk.Checked != false)
                {
                    testInRequest.TestId = tchk.TestId;                   // testInRequest.RequestId = 1; // would need to automate this later                    
                    testInRequest.RequestId = varId;
                    testInRequest.Status = ACTIVITYSTATUS.NEW.ToString();
                    db.TestInRequest.Add(testInRequest);
                    db.SaveChanges();
                    //get the total cost of the test(s) checked
                    var tchk_cost = db.Test.Where(x => x.Id == tchk.TestId).Select(x => x.Cost).FirstOrDefault();
                    cost = cost + tchk_cost;
                }
            }
            var c = db.Request.Where(x => x.ID == varId).FirstOrDefault();
            c.Cost = Convert.ToDecimal(cost);
            //c.Cost = no * Convert.ToDecimal(cost);
            db.SaveChanges();

            return RedirectToAction("Index", "TestInRequest");
        }

        // GET: TestCheck/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCheck testCheck = db.TestCheck.Find(id);
            if (testCheck == null)
            {
                return HttpNotFound();
            }
            return View(testCheck);
        }

        // GET: TestCheck/Create
        public ActionResult Create()
        {
            ViewBag.TestId = new SelectList(db.Test, "Id", "Name");
            return View();
        }

        // POST: TestCheck/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,TestId,Checked")] TestCheck testCheck)
        {
            if (ModelState.IsValid)
            {
                var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());
                //barcodecs objbar = new barcodecs();

                if (currentUser != null)
                    testCheck.createdBy = currentUser.UserName;
                else
                    testCheck.createdBy = User.Identity.Name;

                testCheck.createdDate = DateTime.Now;


                db.TestCheck.Add(testCheck);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TestId = new SelectList(db.Test, "Id", "Name", testCheck.TestId);
            return View(testCheck);
        }

        // GET: TestCheck/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCheck testCheck = db.TestCheck.Find(id);
            if (testCheck == null)
            {
                return HttpNotFound();
            }
            ViewBag.TestId = new SelectList(db.Test, "Id", "Name", testCheck.TestId);
            return View(testCheck);
        }

        // POST: TestCheck/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,TestId,Checked")] TestCheck testCheck)
        {
            if (ModelState.IsValid)
            {

                var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (currentUser != null)
                    testCheck.updatedBy = currentUser.UserName;
                else
                    testCheck.updatedBy = User.Identity.Name;

                testCheck.updatedDate = DateTime.Now;

                db.Entry(testCheck).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TestId = new SelectList(db.Test, "Id", "Name", testCheck.TestId);
            return View(testCheck);
        }

        // GET: TestCheck/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCheck testCheck = db.TestCheck.Find(id);
            if (testCheck == null)
            {
                return HttpNotFound();
            }
            return View(testCheck);
        }

        // POST: TestCheck/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestCheck testCheck = db.TestCheck.Find(id);
            db.TestCheck.Remove(testCheck);
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


//  Request req = new Request();


//var subdatex = db.SubCultures.Where(x => x.varietyId == subculture.varietyId).Select(x => x.currentProcessDate).Max();

//var c = (from x in db.Request
//         where x.ID == varId
//         select x).ToList();
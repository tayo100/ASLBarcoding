using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASLBarcoding.Models;
using DYMO.Label.Framework;
using DYMO;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Microsoft.AspNet.Identity;

namespace ASLBarcoding.Controllers
{
    public class SamplesController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();

        // GET: Samples
        public ActionResult Index()
        {
            var sample = db.Sample.Include(s => s.Request);
            return View(sample.ToList());
        }

        public ActionResult IndexDataList(long? copies, long? id = 0)
        {
            List<CustomSampleModel> customSub = new List<CustomSampleModel>();

            // var sample = db.Sample.Where(x => x.ID == id);

            var sample = from r in db.Sample select r;

            sample = sample.Where(rg => (rg.ID == id));
            var sampleList = sample.ToList();

            if (sampleList == null)
            {
                return Json("No matching record(s) found using your query to the expected generate report! NULL");
            }
            else if (sampleList.Count() <= 0)
            {
                return Json("No matching record(s) found using your query to the expected generate report! EMPTY");
            }

            if (copies > 1)
            {
                //Create a number of copies for the array of request

                for (int i = 0; i < copies; i++)
                {
                    foreach (var q in sampleList)
                    {
                        CustomSampleModel csm = new CustomSampleModel();
                        csm.sampleId = q.ID;
                        csm.sampleNo = q.SampleNo.ToString();
                        csm.barcode = q.Barcode;
                        csm.sampleType = q.Request.SampleType.Name;
                        csm.workOrderNo = q.Request.WorkorderNo.ToString();
                        customSub.Add(csm);
                    }
                }
            }
            else
            {
                foreach (var q in sampleList)
                {
                    CustomSampleModel csm = new CustomSampleModel();
                    csm.sampleId = q.ID;
                    csm.sampleNo = q.SampleNo.ToString();
                    csm.barcode = q.Barcode;
                    csm.sampleType = q.Request.SampleType.Name;
                    csm.workOrderNo = q.Request.WorkorderNo.ToString();
                    customSub.Add(csm);
                }
            }
            return Json(customSub);
        }


        public ActionResult ToExcel(int sampleId)
        {
            var sample = db.Sample.Where(r => r.ID == sampleId).FirstOrDefault();

            var gv = new GridView();
            gv.DataSource = sampleId; //this.GetEmployeeList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Details");
        }



        // GET: Samples/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sample sample = db.Sample.Find(id);
            if (sample == null)
            {
                return HttpNotFound();
            }

            ViewBag.barcode = sample.BarcodeImageUrl;
            return View(sample);
        }

        // GET: Samples/Create
        public ActionResult Create()
        {
            ViewBag.RequestID = new SelectList(db.Request, "ID", "WorkorderNo");
            return View();
        }

        // POST: Samples/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "SampleNo,RequestID")] Sample sample) //Barcode,BarcodeImageUrl,
        {
            if (ModelState.IsValid)
            {
                var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());
                //barcodecs objbar = new barcodecs();

                if (currentUser != null)
                    sample.createdBy = currentUser.UserName;
                else
                    sample.createdBy = User.Identity.Name;

                sample.createdDate = DateTime.Now;


                db.Sample.Add(sample);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RequestID = new SelectList(db.Request, "ID", "ID", sample.RequestID);
            return View(sample);
        }

        // GET: Samples/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sample sample = db.Sample.Find(id);
            if (sample == null)
            {
                return HttpNotFound();
            }
            ViewBag.RequestID = new SelectList(db.Request, "ID", "ID", sample.RequestID);
            return View(sample);
        }

        // POST: Samples/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ID,SampleNo,Barcode,BarcodeImageUrl,RequestID")] Sample sample)
        {
            if (ModelState.IsValid)
            {
                
                var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (currentUser != null)
                    sample.updatedBy = currentUser.UserName;
                else
                    sample.updatedBy = User.Identity.Name;

                sample.updatedDate = DateTime.Now;

                db.Entry(sample).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RequestID = new SelectList(db.Request, "ID", "ID", sample.RequestID);
            return View(sample);
        }

        // GET: Samples/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sample sample = db.Sample.Find(id);
            if (sample == null)
            {
                return HttpNotFound();
            }
            return View(sample);
        }

        // POST: Samples/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Sample sample = db.Sample.Find(id);
            db.Sample.Remove(sample);
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

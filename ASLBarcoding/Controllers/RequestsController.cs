using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASLBarcoding.Models;
using System.Diagnostics;
using BarCode.Models;

namespace ASLBarcoding.Controllers
{
    public class RequestController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();

        // GET: Request
        public ActionResult Index()
        {
            var Request = db.Request.Include(r => r.Client).Include(r => r.SampleType);
            return View(Request.ToList());
        }

        public PartialViewResult All()
        {
            List<TestCheck> model = db.TestCheck.ToList();
            return PartialView("_TestCheck", model);
        }

        public PartialViewResult Soil()
        {
            List<TestCheck> model = db.TestCheck.Where(x => x.Test.TestTypeId == 1).ToList();
            return PartialView("_TestCheck", model);
        }

        public PartialViewResult PlantTissue()
        {
            List<TestCheck> model = db.TestCheck.Where(x => x.Test.TestTypeId == 2).ToList();
            return PartialView("_TestCheck", model);
        }

        public PartialViewResult Seed()
        {
            List<TestCheck> model = db.TestCheck.Where(x => x.Test.TestTypeId == 5).ToList();
            return PartialView("_TestCheck", model);
        }

        public PartialViewResult RootTuber()
        {
            List<TestCheck> model = db.TestCheck.Where(x => x.Test.TestTypeId == 3).ToList();
            return PartialView("_TestCheck", model);
        }

        public PartialViewResult Fertilizer()
        {
            List<TestCheck> model = db.TestCheck.Where(x => x.Test.TestTypeId == 2).ToList();
            return PartialView("_TestCheck", model);
        }

        public PartialViewResult Water()
        {
            List<TestCheck> model = db.TestCheck.Where(x => x.Test.TestTypeId == 4).ToList();
            return PartialView("_TestCheck", model);
        }


        public ActionResult IndexDataList(long? copies, long? id = 0)
        {
            List<CustomSampleModel> customSub = new List<CustomSampleModel>();
            var sample = db.Sample.Where(x => x.ID == id);

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

        // GET: Request/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Request.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: Request/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.Client, "Id", "Name");
            ViewBag.SampleTypeId = new SelectList(db.SampleType, "Id", "Name");
            return View();
        }

        // POST: Request/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cost,NoofSamples,ClientId,CompletionDate,SampleTypeId,RequiredDate,SubmitDate")] Request request) //WorkorderNo,SampleFirstNo,SampleLastNo,
        {
            if (ModelState.IsValid)
            {
                request.WorkorderNo = 2017153;
                request.SampleFirstNo = 201706032;
                request.SampleLastNo = 201706033;

                var worder = db.Request.Select(x => x.WorkorderNo).FirstOrDefault();
                if(worder != 0)
                {
                    var lastWorkOrder = db.Request.Select(x => x.WorkorderNo).Max();
                    request.WorkorderNo = lastWorkOrder + 1;

                    var lstSamplelastNo = db.Request.Select(x => x.SampleLastNo).Max();
                    var toAdd = request.NoofSamples;
                    var newSampleFirstNo = lstSamplelastNo + 1;
                    var newSampleLastNo = lstSamplelastNo + toAdd;

                    request.SampleFirstNo = newSampleFirstNo; 
                    request.SampleLastNo = newSampleLastNo;

                }

                db.Request.Add(request);
                db.SaveChanges();

                //get the total cost 




                // filter the test to pick based on type
                var rId = request.SampleTypeId;
                var ttype = db.SampleType.Where(x => x.Id == rId).Select(x => x.TestType_Id).FirstOrDefault();
                
                TempData["ttype"] = ttype;
                

                var varId = db.Request.Where(x => x.SampleLastNo == request.SampleLastNo).Select(x => x.ID).FirstOrDefault();
                TempData["reqId"] = varId;
                barcodecs objbar = new barcodecs();
                Sample sample = new Sample();


                for (int i = request.SampleFirstNo; i <= request.SampleLastNo; i++)
                {
                    sample.SampleNo = i;
                    sample.Barcode = objbar.generateBarcode(request.WorkorderNo.ToString().Substring(4), i.ToString().Substring(4));
                    sample.BarcodeImageUrl = objbar.getBarcodeImage(sample.Barcode, request.WorkorderNo.ToString().Substring(4) + i.ToString().Substring(4));
                    sample.RequestID = varId;
                    db.Sample.Add(sample);
                    db.SaveChanges();
                }

                


                return RedirectToAction("Index", "TestCheck");
            }

            ViewBag.ClientId = new SelectList(db.Client, "Id", "Name", request.ClientId);
            ViewBag.SampleTypeId = new SelectList(db.SampleType, "Id", "Id", request.SampleTypeId);
            return View(request);
        }

        // GET: Request/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Request.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Client, "Id", "Name", request.ClientId);
            ViewBag.SampleTypeId = new SelectList(db.SampleType, "Id", "Id", request.SampleTypeId);
            return View(request);
        }

        // POST: Request/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,WorkorderNo,Cost,NoofSamples,SampleFirstNo,SampleLastNo,ClientId,CompletionDate,SampleTypeId")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Client, "Id", "Name", request.ClientId);
            ViewBag.SampleTypeId = new SelectList(db.SampleType, "Id", "Id", request.SampleTypeId);
            return View(request);
        }

        // GET: Request/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Request.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Request request = db.Request.Find(id);

            var list = db.TestInRequest.Where(x  => x.RequestId == id).ToList();
            foreach (var item in list)
            {
                db.TestInRequest.Remove(item);
            }

            var list2 = db.Sample.Where(x => x.RequestID == id).ToList();
            foreach (var item in list2)
            {
                db.Sample.Remove(item);
            }

            db.Request.Remove(request);
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

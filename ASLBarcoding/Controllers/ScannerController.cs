using ASLBarcoding.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace YHRSys.Controllers
{
    public class ScannerController : Controller
    {
        private ASLBarcodingDBEntities db = new ASLBarcodingDBEntities();
        //
        // GET: /Scanner/

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TestInRequest testInRequest = db.TestInRequest.Find(id);
            testInRequest.Status = ACTIVITYSTATUS.InProcess.ToString();
            db.SaveChanges();

            if (testInRequest == null)
            {
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(string barcodeText)
        {
            if (barcodeText != null)
            {
                int x = 0;
                if (Int32.TryParse(barcodeText, out x))
                    x = Int32.Parse(barcodeText);

                var sample = from r in db.Sample select r;
                sample = sample.Where(rg => rg.Barcode.Contains(barcodeText) || rg.ID == x);

                if (sample != null)
                {
                    List<Sample> vpf = sample.AsEnumerable().ToList();//.FirstOrDefault();
                    if (vpf != null && vpf.Count() > 0)
                        if (vpf.Count() == 1)
                        {
                            Sample vp = vpf.FirstOrDefault();
                            //                                return RedirectToAction("ToExcel", "Samples", new { id = vp.ID });

                            return RedirectToAction("Details", "Samples", new { id = vp.ID });

                        }
                        else
                            return RedirectToAction("BarcodeDataList", "Samples", new { barcode = barcodeText });
                    else
                        ViewBag.Message = "No record found!";

                }

            }
            return View();
        }
    }
}
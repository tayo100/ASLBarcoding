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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

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

        // GET: /VarietyProcessFlow/
        public ActionResult BarcodeDataList(string barcode)
        {
            var sample = from r in db.Sample select r;

            if (!String.IsNullOrEmpty(barcode))
            {
                sample = sample.Where(rg => rg.Barcode.Contains(barcode));
            }

            return View(sample.ToList());
        }

        //[Authorize(Roles = "Admin, CanViewCummulativeReport, PartnerReporting")]
        [HttpPost]
        public ActionResult ExportDetails(int? id=0)
        {
            /*var sample = db.Sample
                        .Join(db.Request, 
                              a => a.RequestID,
                              b => b.ID, 
                              (a, b) => new { Sample = a, Request = b })                
                        .Join(db.TestInRequest, 
                              a => a.Request.ID,
                              c => c.RequestId,
                              (a, c) => new { a.Request, a.Sample, TestInRequest = c })
                        .Join(db.AspNetUser, 
                              c => c.TestInRequest.UserId,
                              d => d.Id,
                              (c, d) => new { c.Request, c.Sample, c.TestInRequest, AspNetUser = d })
                        .Where(a => a.Sample.ID == id)
                        .Select(a => new RequestSampleReport
                                     {
                                         ReqID = a.Request.ID,
                                        ReqSubmitDate = a.Request.SubmitDate,
                                        ReqRequiredDate = a.Request.RequiredDate,
                                        ReqClientId = a.Request.ClientId,
                                        ReqNoofSamples = a.Request.NoofSamples,
                                        ReqSampleTypeId = a.Request.SampleTypeId,
                                        ReqWorkorderNo = a.Request.WorkorderNo,
                                        ReqCost = a.Request.Cost,
                                        ReqCompletionDate = a.Request.CompletionDate,
                                        ReqSampleFirstNo = a.Request.SampleFirstNo,
                                        ReqSampleLastNo = a.Request.SampleLastNo,
                                        ReqStatus = a.Request.Status,
                                        SampID = a.Sample.ID,
                                        SampSampleNo = a.Sample.SampleNo,
                                        SampBarcode = a.Sample.Barcode,
                                        SampOiC = a.AspNetUser.UserName
                                     });*/

            var sample = from s in db.Sample
                                    join r in db.Request 
                                        on s.RequestID equals r.ID 
                                    join tr in db.TestInRequest 
                                        on r.ID equals tr.RequestId 
                                    join u in db.AspNetUser 
                                        on tr.UserId equals u.Id into aj 
                                    from a in aj.DefaultIfEmpty()
                                    where s.ID == id
                                    select new RequestSampleReport   {
                                        ReqID = s.RequestID,
                                        ReqSubmitDate = r.SubmitDate,
                                        ReqRequiredDate = r.RequiredDate,
                                        ReqClientId = r.ClientId,
                                        ReqNoofSamples = r.NoofSamples,
                                        ReqSampleTypeId = r.SampleTypeId,
                                        ReqWorkorderNo = r.WorkorderNo,
                                        ReqCost = r.Cost,
                                        ReqCompletionDate = r.CompletionDate,
                                        ReqSampleFirstNo = r.SampleFirstNo,
                                        ReqSampleLastNo = r.SampleLastNo,
                                        ReqStatus = r.Status,
                                        SampID = s.RequestID,
                                        SampSampleNo = s.SampleNo,
                                        SampBarcode = s.Barcode,
                                        SampOiC = a.FullName
                                    };

            GridView gv = new GridView();
            gv.DataSource = sample.ToList();
            gv.DataBind();
            DataTable dt = new DataTable();
            //Console.WriteLine(gv.Rows[0].Cells[0].Text);
            dt.Columns.Add("Header");
            dt.Rows.Add(gv.Rows[0].Cells[0].Text);
            dt.Rows.Add(gv.Rows[0].Cells[1].Text);
            dt.Rows.Add(gv.Rows[0].Cells[2].Text);
            dt.Rows.Add(gv.Rows[0].Cells[3].Text);
            dt.Rows.Add(gv.Rows[0].Cells[4].Text);
            dt.Rows.Add(gv.Rows[0].Cells[5].Text);
            dt.Rows.Add(gv.Rows[0].Cells[6].Text);
            dt.Rows.Add(gv.Rows[0].Cells[7].Text);
            dt.Rows.Add(gv.Rows[0].Cells[8].Text);
            dt.Rows.Add(gv.Rows[0].Cells[9].Text);
            dt.Rows.Add(gv.Rows[0].Cells[10].Text);
            dt.Rows.Add(gv.Rows[0].Cells[11].Text);
            dt.Rows.Add(gv.Rows[0].Cells[12].Text);
            dt.Rows.Add(gv.Rows[0].Cells[13].Text);
            dt.Rows.Add(gv.Rows[0].Cells[14].Text);

            string filename = "~/Content/Content/templates/WS00-OC.xls";

            int index = filename.LastIndexOf('.');
            string onlyName = filename.Substring(0, index);
            string extension = filename.Substring(index + 1);

            int indexFN = onlyName.LastIndexOf('/');
            string onlyFname = onlyName.Substring(0, indexFN);
            string onlyFileName = onlyName.Substring(indexFN + 1);
           
            IWorkbook workbook;
            FileStream fileStream = new FileStream(Server.MapPath(filename), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            else if (extension == "xls")
            {
                workbook = new HSSFWorkbook(fileStream);
            }
            else
            {
                throw new Exception("This format is not supported");
            }

            WriteExcelWithNPOI(dt, workbook, extension, onlyFileName);


            return RedirectToAction("Index");

        }

        public void WriteExcelWithNPOI(DataTable dt, IWorkbook workbook, string extension, string onlyName){
            /*ISheet sheet1 = workbook.CreateSheet("Sheet 1");

            //make a header row
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

            //loops through data
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                }
            }*/

            ISheet sheet1 = workbook.GetSheetAt(0);

            IRow row = sheet1.GetRow(3);
            ICell cell = row.CreateCell(1);
            cell.SetCellValue(dt.Rows[6][0].ToString());

            row = sheet1.GetRow(4);
            cell = row.CreateCell(1);
            cell.SetCellValue(dt.Rows[13][0].ToString());

            row = sheet1.GetRow(5);
            cell = row.CreateCell(1);
            cell.SetCellValue(dt.Rows[14][0].ToString());

            row = sheet1.GetRow(6);
            cell = row.CreateCell(1);
            cell.SetCellValue(string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now));
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                if (extension == "xlsx") //xlsx file format
                {
                    string excelFileName = onlyName + "_" + string.Format("{0:dd_MM_yyyy_HH_mm_ss}", DateTime.Now) + ".xlsx";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", excelFileName));
                    Response.BinaryWrite(exportData.ToArray());
                }
                else if (extension == "xls")  //xls file format
                {
                    string excelFileName = onlyName + "_" + string.Format("{0:dd_MM_yyyy_HH_mm_ss}", DateTime.Now) + ".xls";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", excelFileName));
                    Response.BinaryWrite(exportData.GetBuffer());
                }
                Response.End();
            }
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

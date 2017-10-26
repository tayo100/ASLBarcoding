using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;

namespace BarCode.Models
{
    public class barcodecs
    {
        public string generateBarcode(string workOrderNo, string sampleNo)
        {
            try
            {

                StringBuilder rs = new StringBuilder();

                //  var code = "";
                //  var accessionNo = "";

              //  var sampleParts = sampleNo.Substring(4);

                //if (!varietyParts[0].Equals("") && !varietyParts[1].Equals(""))
                //{
                //      //Drop the first letter; if TDr or TDa it will now be Dr or Da
                        // 
                //      if (varietyParts[0].Length < 3)
                //        {
                //            code = varietyParts[0];
                //        }
                //      else if (varietyParts[0].Length == 3)
                //      {
                //          code = varietyParts[0].Substring(1);
                //      }
                //      else {
                //          code = varietyParts[0].Substring(0,2);
                //        }

                //      String[] spaceVal = varietyParts[1].Split(' ');

                //      if (!spaceVal[0].Equals(""))
                //      {
                //          if (spaceVal[0].IndexOf('-') > 0)
                //          {
                //              String[] dashVal = spaceVal[0].Split('-');

                //              //Remove the year part
                //              if (dashVal[0].IndexOf("/") > 0)
                //              {
                //                  String[] slashVal = dashVal[0].Split('/');
                //                  if (slashVal.Length > 1 )
                //                  {
                //                      if (!slashVal[1].Equals(""))
                //                          accessionNo = slashVal[1];
                //                      else
                //                          accessionNo = slashVal[0];
                //                  }
                //                      else
                //                      accessionNo = slashVal[0];
                //              }
                //              else
                //              {
                //                  accessionNo = dashVal[0];
                //              }                            
                //          }
                //          else
                //          {
                //              //Remove the year part
                //              String[] slashVal = spaceVal[0].Split('/');
                //              if (slashVal.Length > 1 )
                //              {
                //                  if (!slashVal[1].Equals(""))
                //                      accessionNo = slashVal[1];
                //                  else
                //                      accessionNo = slashVal[0];
                //              }
                //              else
                //                  accessionNo = slashVal[0];
                //          }
                //      }
                //    workOrderNo = code + "" + accessionNo;
                //}
                //else if (!varietyParts[0].Equals(""))
                //{
                //    if (varietyParts[0].Length < 3)
                //    {
                //        workOrderNo = varietyParts[0];
                //    }
                //    else if (varietyParts[0].Length == 3)
                //    {
                //        if (varietyParts[0].Equals("TDr") || varietyParts[0].Equals("TDa"))
                //            workOrderNo = varietyParts[0].Substring(1);
                //        else
                //            workOrderNo = varietyParts[0];
                //    }
                //    else
                //    {
                //        workOrderNo = varietyParts[0].Substring(0, 2);
                //    }
                //}

                rs.Append(workOrderNo.Replace("(", "/").Replace(")", "/").Replace(" ", "").ToUpper()).Append("/").Append(sampleNo.Replace(" ", "").ToUpper());
                return rs.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Barcode: " + ex.ToString() + "  " + ex.Message);
            }
            return "";
        }  

        public Byte[] getBarcodeImage(string barcode, string file)
        {
            try
            {
                BarCode39 _barcode = new BarCode39();
                int barSize = 10;
                string fontFile = HttpContext.Current.Server.MapPath("~/fonts/free3of9.ttf");
                return (_barcode.Code39(barcode, barSize, false, file, fontFile));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Barcode: " + ex.ToString() + "  " + ex.Message);
            }
            return null;
        }
    }
}

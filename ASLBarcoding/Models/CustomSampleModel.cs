using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASLBarcoding.Models
{
    public class CustomSampleModel
    {
        public Decimal sampleId { get; set; }
        public String sampleType { get; set; }
        public String workOrderNo { get; set; }
        public String sampleNo { get; set; }        
        public String barcode { get; set; }
        public String barcodeImageUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASLBarcoding.Models
{
    public class RequestSampleReport
    {
        public Int32? ReqID { get; set; }
        public DateTime? ReqSubmitDate { get; set; }
        public DateTime? ReqRequiredDate { get; set; }
        public Int32? ReqClientId { get; set; }
        public Int32? ReqNoofSamples { get; set; }
        public Int32? ReqSampleTypeId { get; set; }
        public Int32? ReqWorkorderNo { get; set; }
        public Decimal ReqCost { get; set; }
        public DateTime? ReqCompletionDate { get; set; }
        public Int32? ReqSampleFirstNo { get; set; }
        public Int32? ReqSampleLastNo { get; set; }
        public String ReqStatus { get; set; }
        public String SampOiC { get; set; }

        public Int32? SampID { get; set; }
        public Int32? SampSampleNo { get; set; }
        public String SampBarcode { get; set; }
    }
}
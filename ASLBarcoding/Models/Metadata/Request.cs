using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASLBarcoding.Models
{
    [MetadataType(typeof(RequestMetadata))]
    public partial class Request
    {
    }


    public class RequestMetadata
    {
        //[DisplayName("Date-Last-Subcultured")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> SubmitDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RequiredDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CompletionDate { get; set; }
    }
}
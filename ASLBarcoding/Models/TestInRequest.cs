//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASLBarcoding.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TestInRequest
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int TestId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public Nullable<System.DateTime> updatedDate { get; set; }
        public byte[] Timestamp { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
    
        public virtual Request Request { get; set; }
        public virtual Test Test { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}

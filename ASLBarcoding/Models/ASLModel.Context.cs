﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ASLBarcodingDBEntities : DbContext
    {
        public ASLBarcodingDBEntities()
            : base("name=ASLBarcodingDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Analysis> Analysis { get; set; }
        public virtual DbSet<AspNetRole> AspNetRole { get; set; }
        public virtual DbSet<AspNetUser> AspNetUser { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRole { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<Result> Result { get; set; }
        public virtual DbSet<Sample> Sample { get; set; }
        public virtual DbSet<SampleType> SampleType { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestCheckList> TestCheckList { get; set; }
        public virtual DbSet<TestInRequest> TestInRequest { get; set; }
        public virtual DbSet<TestType> TestType { get; set; }
        public virtual DbSet<TestCheck> TestCheck { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
    }
}
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArenaTest.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ArenaTestModelsContainer : DbContext
    {
        public ArenaTestModelsContainer()
            : base("name=ArenaTestModelsContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TestCase> TestCases { get; set; }
        public virtual DbSet<TestRun> TestRuns { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
    }
}

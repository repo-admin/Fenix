﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fenix
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FenixEntities : DbContext
    {
        public FenixEntities()
            : base("name=FenixEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CardStockItems> CardStockItems { get; set; }
        public virtual DbSet<InternalDocuments> InternalDocuments { get; set; }
        public virtual DbSet<InternalDocumentsSources> InternalDocumentsSources { get; set; }
        public virtual DbSet<cdlMessageNumber> cdlMessageNumber { get; set; }
        public virtual DbSet<CommunicationMessagesDeleteMessage> CommunicationMessagesDeleteMessage { get; set; }
    }
}
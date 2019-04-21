using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Moments.Models;

namespace Moments.dal
{
    public class messagesDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Messages>().ToTable("Messages");
        }
        public DbSet<Messages> MessagesLst { get; set; }
    }
}
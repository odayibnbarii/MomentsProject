using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;


namespace Moments.dal
{
    public class momentsDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<moments>().ToTable("moments");
        }
        public DbSet<moments> momentsLst { get; set; }
    }
}
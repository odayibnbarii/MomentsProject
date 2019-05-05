using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;
namespace Moments.dal
{
    public class websiteImages : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<adminPhoto>().ToTable("websiteImages");
        }
        public DbSet<adminPhoto> pLst { get; set; }
    }
}
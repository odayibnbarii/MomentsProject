using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;
namespace Moments.dal
{
    public class adminsDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<admins>().ToTable("admins");
        }
        public DbSet<admins> adminsLst { get; set; }
    }
}
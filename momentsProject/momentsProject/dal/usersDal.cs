using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;
namespace Moments.dal
{
    public class usersDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<users>().ToTable("users");
        }
        public DbSet<users> userLst { get; set; }

        //public System.Data.Entity.DbSet<Moments.Models.moments> moments { get; set; }
    }
}
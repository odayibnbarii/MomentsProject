using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;

namespace Moments.dal
{
    public class profileDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Profile>().ToTable("profiles");
        }
        public DbSet<Profile> profilesList { get; set; }
    }
}
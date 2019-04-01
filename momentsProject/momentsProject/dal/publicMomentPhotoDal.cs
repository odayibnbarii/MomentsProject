using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;

namespace Moments.dal
{
    public class publicMomentPhotoDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<publicMomentPhoto>().ToTable("publicMomentPhoto");
        }
        public DbSet<publicMomentPhoto> momentPhotoLst { get; set; }
    }
}
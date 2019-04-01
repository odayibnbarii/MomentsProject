using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Moments.Models;

namespace Moments.dal
{
    public class momentPhotoDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<momentPhoto>().ToTable("momentPhoto");
        }
        public DbSet<momentPhoto> momentPhotoLst { get; set; }
    }
}
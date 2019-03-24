using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Moments.Models;
namespace Moments.dal
{
    public class friendsDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Friends>().ToTable("friends");
        }
        public DbSet<Friends> FriendsLst { get; set; }
    }
}
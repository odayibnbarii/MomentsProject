using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;

namespace Moments.dal
{
    public class UsersStatusDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<usersStatus>().ToTable("usersStatus");
        }
        public DbSet<usersStatus> statuses { get; set; }
    }
}
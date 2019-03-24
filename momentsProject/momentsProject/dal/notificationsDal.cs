using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Moments.Models;

namespace Moments.dal
{
    [Table("ConferenceLogin")]
    public class notificationsDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Notifications>().ToTable("notifications");
        }
        public DbSet<Notifications> nLst { get; set; }
    }
}
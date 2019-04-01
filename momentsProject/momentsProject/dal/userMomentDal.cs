
using System.Data.Entity;


namespace Moments.Models
{
    public class userMomentDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<userMoments>().ToTable("userMoments");
        }
        public DbSet<userMoments> userMomentLST { get; set; }
    }
}
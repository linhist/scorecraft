using System.Data.Entity;

namespace Scorecraft.Data
{
    public class SCDbContext : DbContext
    {
        public SCDbContext()
            : base("DB.SCORECRAFT")
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SofaEntity>();
        }
    }
}
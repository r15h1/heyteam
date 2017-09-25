using HeyTeam.Lib.Data;
using Microsoft.EntityFrameworkCore;

namespace HeyTeam.Lib.Data {
    public class ClubContext : DbContext {
        public ClubContext(DbContextOptions options) : base(options) { }        

        public DbSet<Club> Clubs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Data.Club>().HasKey(f => f.Id);                
            modelBuilder.Entity<Data.Club>().Property(f => f.Id).ValueGeneratedOnAdd();
        }
    }
}
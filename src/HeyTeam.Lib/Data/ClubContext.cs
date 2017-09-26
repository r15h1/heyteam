using HeyTeam.Lib.Data;
using Microsoft.EntityFrameworkCore;

namespace HeyTeam.Lib.Data {
    public class ClubContext : DbContext {
        public ClubContext(DbContextOptions options) : base(options) { }        

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Squad> Squads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Data.Club>().HasKey(f => f.ClubId);                            
            modelBuilder.Entity<Data.Club>().Property(f => f.ClubId).ValueGeneratedOnAdd();            
            modelBuilder.Entity<Data.Club>().HasAlternateKey(f => f.Guid);            

            modelBuilder.Entity<Data.Squad>().HasKey(f => f.SquadId);                            
            modelBuilder.Entity<Data.Squad>().Property(f => f.SquadId).ValueGeneratedOnAdd();            
            modelBuilder.Entity<Data.Squad>().HasAlternateKey(f => f.Guid);

            modelBuilder.Entity<Data.Club>()
                .HasMany(s => s.Squads)
                .WithOne(c => c.Club)
                .HasForeignKey(s => s.ClubId);
        }
    }
}
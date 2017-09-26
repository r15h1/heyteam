using System;
using HeyTeam.Lib.Data;
using Microsoft.EntityFrameworkCore;

namespace HeyTeam.Tests.DataContext {
    public class ClubContextOptionsBuilder : DbContextOptionsBuilder<ClubContext> {
        private readonly DbContextOptionsBuilder<ClubContext> builder;

        public ClubContextOptionsBuilder(){
            var builder = new DbContextOptionsBuilder<ClubContext>();
            this.builder = builder.UseSqlite($"Data Source=file: {Guid.NewGuid().ToString()}.sqlite");
            this.builder.EnableSensitiveDataLogging(true);
        }

        public override DbContextOptions<ClubContext> Options => builder.Options;
    }
}
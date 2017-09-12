using System;
using Xunit;
using HeyTeam.Core.Entities;

namespace HeyTeam.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void DescribePlayer()
        {
            Player player = new Player();
            player.Id = 5;
            player.FirstName = "John";
            player.LastName = "Smith";
            player.Nationality = "Canadian";
            player.SquadNumber = "35";
            player.DominantFoot = Player.Foot.LEFT;
            
            Assert.Equal(player.Id, 5);
            Assert.Same(player.FirstName, "John");
            Assert.Same(player.LastName, "Smith");
            Assert.Same(player.Nationality, "Canadian");
            Assert.Same(player.SquadNumber, "35");
            Assert.Equal(player.DominantFoot, Player.Foot.LEFT);             
        }

        /*[Fact]
        public void PlayerCanEvaluate*/
    }
}
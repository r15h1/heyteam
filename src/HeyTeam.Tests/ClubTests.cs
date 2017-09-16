using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Tests {
    public class ClubTests {
        [Fact]
        public void NullSquadCannotBeAdded() {
            var club = new Club();
            Assert.Throws<ArgumentNullException>(() => club.AddSquad(null));
        }

        [Fact]
        public void SquadNameCannotBeNull() {
            var club = new Club();
            Assert.Throws<IllegalOperationException>(() => club.AddSquad(
                new Squad { Name = null }
            ));
        }

        [Fact]
        public void SquadNameCannotBeEmpty() {
            var club = new Club();
            Assert.Throws<IllegalOperationException>(() => club.AddSquad(
                new Squad { Name = "" }
            ));
        }

        [Fact]
        public void SquadNameCannotBeWhitespace() {
            var club = new Club();
            Assert.Throws<IllegalOperationException>(() => club.AddSquad(
                new Squad { Name = " " }
            ));
        }

        [Fact]
        public void SquadNameWithValidNameCanBeAdded() {
            var club = new Club();
            club.AddSquad(new Squad { Name = "Bro Squad"});
            Assert.True(club.Squads.Count == 1);
        }
    }
}
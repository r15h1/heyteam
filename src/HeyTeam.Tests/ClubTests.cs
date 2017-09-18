using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Tests {
    public class ClubTests {

        private Club GetClub() {
            return new Club(1);
        }

        [Fact]
        public void NullSquadCannotBeAdded() {
            var club = GetClub();
            Assert.Throws<ArgumentNullException>(() => club.AddSquad(null));
        }

        [Fact]
        public void SquadNameCannotBeNull() {
            var club = GetClub();
            Assert.Throws<IllegalOperationException>(() => club.AddSquad(
                new Squad(club) { Name = null }
            ));
        }

        [Fact]
        public void SquadNameCannotBeEmpty() {
            var club = GetClub();
            Assert.Throws<IllegalOperationException>(() => club.AddSquad(
                new Squad(club) { Name = "" }
            ));
        }

        [Fact]
        public void SquadNameCannotBeWhitespace() {
            var club = GetClub();
            Assert.Throws<IllegalOperationException>(() => club.AddSquad(
                new Squad(club) { Name = " " }
            ));
        }

        [Fact]
        public void SquadNameWithValidNameCanBeAdded() {
            var club = GetClub();
            club.AddSquad(new Squad(club) { Name = "Bro Squad"});
            Assert.True(club.Squads.Count == 1);
        }
    }
}
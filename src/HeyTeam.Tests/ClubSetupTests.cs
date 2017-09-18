using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;
using HeyTeam.Lib.Validation;
using HeyTeam.Core.Interactors;
using HeyTeam.Core.Validation;
using HeyTeam.Tests.Repositories;

namespace HeyTeam.Tests {
    public class ClubSetupTests {
        private readonly ClubSetupInteractor interactor;

        public ClubSetupTests() {
            var validator = new ClubSetupValidator();
            var repository = new MockClubRepository();
            this.interactor = new ClubSetupInteractor(repository, validator);
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            ClubSetupRequest request = new ClubSetupRequest { ClubName = null };
            var response = interactor.SetupClub(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            ClubSetupRequest request = new ClubSetupRequest { ClubName = string.Empty };
            var response = interactor.SetupClub(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            ClubSetupRequest request = new ClubSetupRequest { ClubName = "  " };
            var response = interactor.SetupClub(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubWithValidNameIsSaved() {
            ClubSetupRequest request = new ClubSetupRequest { ClubName = "Manchester United" };
            var response = interactor.SetupClub(request);            
            Assert.True(response.ValidationResult.IsValid && response.ClubId.HasValue);
        }

        [Fact]
        public void ClubWithValidNamesAreSaved() {
            ClubSetupRequest request1 = new ClubSetupRequest { ClubName = "Manchester United" };
            var response1 = interactor.SetupClub(request1);            
            Assert.True(response1.ValidationResult.IsValid && response1.ClubId.HasValue);

            ClubSetupRequest request2 = new ClubSetupRequest { ClubName = "FC Barcelona" };
            var response2 = interactor.SetupClub(request2);            
            Assert.True(response2.ValidationResult.IsValid && response2.ClubId.HasValue);
            Assert.True(response1.ClubId.Value != response2.ClubId.Value);
        }
    }
}
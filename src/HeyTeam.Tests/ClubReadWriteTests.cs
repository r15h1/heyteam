using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;
using HeyTeam.Lib.Validation;
using HeyTeam.Core.Interactors;
using HeyTeam.Core.Validation;
using HeyTeam.Tests.Repositories;

namespace HeyTeam.Tests {
    public class ClubReadWriteTests {
        private readonly ClubReadWriteInteractor interactor;

        public ClubReadWriteTests() {
            var validator = new ClubEntityValidator();
            var repository = new MockClubRepository();
            this.interactor = new ClubReadWriteInteractor(repository, validator);
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            SaveClubRequest request = new SaveClubRequest { ClubName = null };
            var response = interactor.Handle(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            SaveClubRequest request = new SaveClubRequest { ClubName = string.Empty };
            var response = interactor.Handle(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            SaveClubRequest request = new SaveClubRequest { ClubName = "  " };
            var response = interactor.Handle(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubWithValidNameIsSaved() {
            SaveClubRequest request = new SaveClubRequest { ClubName = "Manchester United" };
            var response = interactor.Handle(request);            
            Assert.True(response.ValidationResult.IsValid && response.ClubId.HasValue);
        }

        [Fact]
        public void ClubWithValidNamesAreSaved() {
            SaveClubRequest request1 = new SaveClubRequest { ClubName = "Manchester United" };
            var response1 = interactor.Handle(request1);            
            Assert.True(response1.ValidationResult.IsValid && response1.ClubId.HasValue);

            SaveClubRequest request2 = new SaveClubRequest { ClubName = "FC Barcelona" };
            var response2 = interactor.Handle(request2);            
            Assert.True(response2.ValidationResult.IsValid && response2.ClubId.HasValue);
            Assert.True(response1.ClubId.Value != response2.ClubId.Value);
        }
    }
}
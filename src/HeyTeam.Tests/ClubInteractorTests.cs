using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;
using HeyTeam.Lib.Validation;
using HeyTeam.Core.Interactors;
using HeyTeam.Core.Validation;
using HeyTeam.Tests.Repositories;
using HeyTeam.Core.Requests;

namespace HeyTeam.Tests {
    public class ClubInteractorTests {
        private readonly ClubInteractor interactor;

        public ClubInteractorTests() {
            var validator = new ClubSaveRequestValidator();
            var repository = new MockClubRepository();
            this.interactor = new ClubInteractor(repository, validator);
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            ClubSaveRequest request = new ClubSaveRequest { ClubName = null };
            var response = interactor.Handle(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            ClubSaveRequest request = new ClubSaveRequest { ClubName = string.Empty };
            var response = interactor.Handle(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            ClubSaveRequest request = new ClubSaveRequest { ClubName = "  " };
            var response = interactor.Handle(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubWithValidNameIsSaved() {
            ClubSaveRequest request = new ClubSaveRequest { ClubName = "Manchester United" };
            var response = interactor.Handle(request);            
            Assert.True(response.ValidationResult.IsValid && response.ClubId.HasValue);
        }

        [Fact]
        public void ClubWithValidNamesAreSaved() {
            ClubSaveRequest request1 = new ClubSaveRequest { ClubName = "Manchester United" };
            var response1 = interactor.Handle(request1);            
            Assert.True(response1.ValidationResult.IsValid && response1.ClubId.HasValue);

            ClubSaveRequest request2 = new ClubSaveRequest { ClubName = "FC Barcelona" };
            var response2 = interactor.Handle(request2);            
            Assert.True(response2.ValidationResult.IsValid && response2.ClubId.HasValue);
            Assert.True(response1.ClubId.Value != response2.ClubId.Value);
        }
    }
}
using System;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Repositories;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class UpdateClubProfileTests {
        private readonly UpdateClubProfileUseCase useCase;

        public UpdateClubProfileTests() {
            var validator = new UpdateClubProfileRequestValidator();
            var repository = new MockClubRepository();
            this.useCase = new UpdateClubProfileUseCase(repository, validator);
         }

        [Fact]
        public void ClubNameCannotBeNull() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubName = null };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubName = string.Empty };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubName = "  " };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }
    }    
}
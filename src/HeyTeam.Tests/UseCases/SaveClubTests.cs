using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Lib.Validation;
using HeyTeam.Core.Validation;
using HeyTeam.Tests.Repositories;
using HeyTeam.Core.UseCases.Club;

namespace HeyTeam.UsesCases.Tests {
    public class SaveClubTests {
        private readonly SaveClubUseCase useCase;

        public SaveClubTests() {
            var validator = new SaveClubRequestValidator();
            var repository = new MockClubRepository();
            this.useCase = new SaveClubUseCase(repository, validator);
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            SaveClubRequest request = new SaveClubRequest { ClubName = null };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            SaveClubRequest request = new SaveClubRequest { ClubName = string.Empty };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            SaveClubRequest request = new SaveClubRequest { ClubName = "  " };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubWithValidNameIsSaved() {
            SaveClubRequest request = new SaveClubRequest { ClubName = "Manchester United" };
            var response = useCase.Execute(request);            
            Assert.True(response.ValidationResult.IsValid && response.ClubId.HasValue);
        }

        [Fact]
        public void ClubWithValidNamesAreSaved() {
            SaveClubRequest request1 = new SaveClubRequest { ClubName = "Manchester United" };
            var response1 = useCase.Execute(request1);            
            Assert.True(response1.ValidationResult.IsValid && response1.ClubId.HasValue);

            SaveClubRequest request2 = new SaveClubRequest { ClubName = "FC Barcelona" };
            var response2 = useCase.Execute(request2);            
            Assert.True(response2.ValidationResult.IsValid && response2.ClubId.HasValue);
            Assert.True(response1.ClubId.Value != response2.ClubId.Value);
        }
    }
}
using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Lib.Validation;
using HeyTeam.Core.Validation;
using HeyTeam.Tests.Repositories;
using HeyTeam.Core.UseCases.Club;

namespace HeyTeam.Tests.UsesCases {
    public class RegisterClubTests {
        private readonly RegisterClubUseCase useCase;

        public RegisterClubTests() {
            var validator = new RegisterClubRequestValidator();
            var repository = new MockClubRepository();
            this.useCase = new RegisterClubUseCase(repository, validator);
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = null };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = string.Empty };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = "  " };
            var response = useCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubWithValidNameIsSaved() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = "Manchester United" };
            var response = useCase.Execute(request);            
            Assert.True(response.ValidationResult.IsValid && response.ClubId.HasValue);
        }

        [Fact]
        public void TwoClubWithValidNameshaveDifferentIds() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" };
            var response1 = useCase.Execute(request1);            
            Assert.True(response1.ValidationResult.IsValid && response1.ClubId.HasValue);

            RegisterClubRequest request2 = new RegisterClubRequest { ClubName = "FC Barcelona" };
            var response2 = useCase.Execute(request2);            
            Assert.True(response2.ValidationResult.IsValid && response2.ClubId.HasValue);
            Assert.True(response1.ClubId.Value != response2.ClubId.Value);
        }

        [Fact]
        public void LogoUrlRequiresScheme() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "google.com"};
            var response = useCase.Execute(request1);            
            Assert.True(!response.ValidationResult.IsValid);
        }
        
        [Fact]
        public void HttpLogoUrlIsSaved() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "http://google.com"};
            var response = useCase.Execute(request1);            
            Assert.True(response.ValidationResult.IsValid && response.ClubId.HasValue);
        }

        [Fact]
        public void HttpsLogoUrlIsSaved() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "https://google.com"};
            var response = useCase.Execute(request1);            
            Assert.True(response.ValidationResult.IsValid && response.ClubId.HasValue);
        }
    }
}
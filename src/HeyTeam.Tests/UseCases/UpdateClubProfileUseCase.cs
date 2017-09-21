using System;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Repositories;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class UpdateClubProfileTests {
        private readonly MockClubRepository repository;
        private readonly UpdateClubProfileUseCase updateProfileUseCase;
        private readonly RegisterClubUseCase registerUseCase;

        public UpdateClubProfileTests() {
            var validator = new UpdateClubProfileRequestValidator();
            this.repository = new MockClubRepository();
            this.updateProfileUseCase = new UpdateClubProfileUseCase(repository, validator);
            this.registerUseCase = new RegisterClubUseCase(repository, new RegisterClubRequestValidator());
         }

        [Fact]
        public void ClubNameCannotBeNull() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubName = null };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubName = string.Empty };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubName = "  " };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.ValidationResult.IsValid);
        }

                [Fact]
        public void LogoUrlRequiresScheme() {
            RegisterClubRequest registerRequest = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "google.com"};
            var registerResponse = registerUseCase.Execute(registerRequest);            
            Assert.True(!registerResponse.ValidationResult.IsValid && registerResponse.ClubId.HasValue);

            UpdateClubProfileRequest updateRequest = new UpdateClubProfileRequest { ClubId = registerResponse.ClubId.Value,  ClubName = "Manchester United" , ClubLogoUrl = "google.com"};
            var response = updateProfileUseCase.Execute(updateRequest);            
            Assert.True(!response.ValidationResult.IsValid);
        }
        
        [Fact]
        public void HttpLogoUrlIsSaved() {
            UpdateClubProfileRequest request1 = new UpdateClubProfileRequest { ClubName = "Manchester United" , ClubLogoUrl = "http://google.com"};
            var response = updateProfileUseCase.Execute(request1);            
            Assert.True(response.ValidationResult.IsValid);
        }

        [Fact]
        public void HttpsLogoUrlIsSaved() {
            UpdateClubProfileRequest request1 = new UpdateClubProfileRequest { ClubName = "Manchester United" , ClubLogoUrl = "https://google.com"};
            var response = updateProfileUseCase.Execute(request1);            
            Assert.True(response.ValidationResult.IsValid);
        }
    }    
}
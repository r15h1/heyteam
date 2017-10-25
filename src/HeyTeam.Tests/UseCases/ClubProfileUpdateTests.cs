using System;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Data;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class ClubProfileUpdateTests {
        private readonly IClubRepository repository;
        private readonly IUseCase<UpdateClubProfileRequest, Response<Guid>> updateProfileUseCase;
        private readonly Guid clubId;

        public ClubProfileUpdateTests() {
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);                     
            IValidator<UpdateClubProfileRequest> validator = new UpdateClubProfileRequestValidator();
            this.repository = new ClubRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            this.updateProfileUseCase = new UpdateClubProfileUseCase(repository, validator);
            this.clubId = SetupClub();
        }

        private Guid SetupClub() {
            var registerUseCase = new RegisterClubUseCase(repository, new RegisterClubRequestValidator());
            RegisterClubRequest registerRequest = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "http://manutd.com"};
            var registerResponse = registerUseCase.Execute(registerRequest);  
            return registerResponse.Result.Value;
        }

        [Fact]
        public void ClubIdMustBeAssigned() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubName = "Valid name" };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void InexistentClubUpdateThrowsClubNotFoundException() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = Guid.NewGuid(), ClubName = "Manchester United" , ClubLogoUrl = "https://google.com"};            
            var response = updateProfileUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1 && typeof(ClubNotFoundException) == response.Exception.GetType());
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId, ClubName = null };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId, ClubName = string.Empty };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId, ClubName = "  " };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void LogoUrlRequiresScheme() {
            UpdateClubProfileRequest updateRequest = new UpdateClubProfileRequest { ClubId = clubId,  ClubName = "Manchester United" , ClubLogoUrl = "google.com"};
            var response = updateProfileUseCase.Execute(updateRequest);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }
        
        [Fact]
        public void NameUpdateIsSaved() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId, ClubName = "Barcelona", ClubLogoUrl = "http://manutd.com" };
            var response = updateProfileUseCase.Execute(request);
            var club = repository.Get(clubId);
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Barcelona", "http://manutd.com");
        }

        [Fact]
        public void HttpLogoUrlUpdateIsSaved() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId, ClubName = "Manchester United", ClubLogoUrl = "http://google.com" };
            var response = updateProfileUseCase.Execute(request);
            var club = repository.Get(clubId);
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Manchester United", "http://google.com");
        }

        [Fact]
        public void HttpsLogoUrlIsSaved() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId, ClubName = "Manchester United" , ClubLogoUrl = "https://google.com"};
            var response = updateProfileUseCase.Execute(request);  
            var club = repository.Get(clubId);          
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Manchester United", "https://google.com");
        }

        [Fact]
        public void BothNameAndLogoAreUpdated() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId, ClubName = "Barcelona" , ClubLogoUrl = "https://www.fcbarcelona.com"};
            var response = updateProfileUseCase.Execute(request);  
            var club = repository.Get(clubId);          
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Barcelona", "https://www.fcbarcelona.com");
        }

        private void CheckNameAndLogo(Core.Entities.Club club, string name, string logoUrl) {
            Assert.True(club.LogoUrl.Equals(logoUrl));
            Assert.True(club.Name.Equals(name));
        }
    }    
}
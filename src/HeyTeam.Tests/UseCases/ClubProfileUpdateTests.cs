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
using HeyTeam.Util;
using Xunit;
using static HeyTeam.Core.UseCases.Club.UpdateClubProfileRequest;

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
            this.clubId = SetupClub("Manchester United", "http://manutd.com");
        }

        private Guid SetupClub(string name, string url) {
            var registerUseCase = new RegisterClubUseCase(repository, new RegisterClubRequestValidator());
            RegisterClubRequest registerRequest = new RegisterClubRequest { Name = name , Url = url};
            var registerResponse = registerUseCase.Execute(registerRequest);  
            return registerResponse.Result.Value;
        }

        [Fact]
        public void ClubIdMustBeValid() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = Guid.Empty };
            request.SetFieldValue(UpdatableFields.NAME, "Manchester United");
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void InexistentClubUpdateThrowsClubNotFoundException() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = Guid.NewGuid() };
            request.SetFieldValue(UpdatableFields.NAME, "Manchester United");
            request.SetFieldValue(UpdatableFields.URL, "http://manutd.com");

            var response = updateProfileUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1 && typeof(ClubNotFoundException) == response.Exception.GetType());
        }

        [Fact]
        public void AtLeastOneUpdateableFieldMustBeSpecified() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.NAME, null);
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.NAME, string.Empty);
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.NAME, "  ");
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void LogoUrlRequiresScheme() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.URL, "manutd.com");
            var response = updateProfileUseCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }
        
        [Fact]
        public void NameUpdateIsSaved() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.NAME, "Barcelona");
            var response = updateProfileUseCase.Execute(request);
            var club = repository.Get(clubId);
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Barcelona", "http://manutd.com");
        }

        [Fact]
        public void HttpLogoUrlUpdateIsSaved() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.URL, "http://barca.com");
            var response = updateProfileUseCase.Execute(request);
            var club = repository.Get(clubId);
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Manchester United", "http://barca.com");
        }

        [Fact]
        public void HttpsLogoUrlIsSaved() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.URL, "https://barca.com");
            var response = updateProfileUseCase.Execute(request);  
            var club = repository.Get(clubId);          
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Manchester United", "https://barca.com");
        }

        [Fact]
        public void BothNameAndLogoAreUpdated() {
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.URL, "https://www.fcbarcelona.com");
            request.SetFieldValue(UpdatableFields.NAME, "Barcelona");

            var response = updateProfileUseCase.Execute(request);  
            var club = repository.Get(clubId);          
            Assert.True(response.WasRequestFulfilled);
            CheckNameAndLogo(club, "Barcelona", "https://www.fcbarcelona.com");
        }

        [Fact]
        public void UrlMustNotBeInUseAlready() {
            SetupClub("Barcelona", "https://www.fcbarcelona.com");

            //try update manutd url to barca url
            UpdateClubProfileRequest request = new UpdateClubProfileRequest { ClubId = clubId };
            request.SetFieldValue(UpdatableFields.URL, "https://www.fcbarcelona.com");
            var response = updateProfileUseCase.Execute(request); 
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1 && typeof(DuplicateEntryException) == response.Exception.GetType());
        }

        private void CheckNameAndLogo(Core.Entities.Club club, string name, string logoUrl) {
            Assert.True(club.Url.Equals(logoUrl));
            Assert.True(club.Name.Equals(name));
        }

        // private UpdateClubProfileRequest BuildRequest(Guid clubId, string name = "Manchester United", string url = "http://manutd.com") {
        //     var request = new UpdateClubProfileRequest { ClubId = clubId };
        //     if(!name.IsEmpty()) 
        //         request.SetFieldValue(UpdatableFields.NAME, name);

        //     if(!url.IsEmpty())
        //         request.SetFieldValue(UpdatableFields.URL, url);            

        //     return request;
        // }
    }    
}
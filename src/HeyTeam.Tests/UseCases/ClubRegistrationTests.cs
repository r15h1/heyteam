using System;
using Xunit;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Validation;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Lib.Data;
using HeyTeam.Tests.Data;

namespace HeyTeam.Tests.UseCases {
    public class ClubRegistrationTests {
        private readonly IUseCase<RegisterClubRequest, RegisterClubResponse> useCase;

        public ClubRegistrationTests() {   
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);         
            IValidator<RegisterClubRequest> validator = new RegisterClubRequestValidator();
            IClubRepository repository = new ClubRepository(new ConnectionFactory(connectionString));
            this.useCase = new RegisterClubUseCase(repository, validator);
        }

        [Fact]
        public void ClubNameCannotBeNull() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = null };
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Messages.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = string.Empty };
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Messages.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = "  " };
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Messages.Count == 1);
        }

        [Fact]
        public void ClubWithValidNameIsSaved() {
            RegisterClubRequest request = new RegisterClubRequest { ClubName = "Manchester United" };
            var response = useCase.Execute(request);            
            Assert.True(response.WasRequestFulfilled && response.ClubId.HasValue);
        }

        [Fact]
        public void TwoClubWithValidNameshaveDifferentIds() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" };
            var response1 = useCase.Execute(request1);            
            Assert.True(response1.WasRequestFulfilled && response1.ClubId.HasValue);

            RegisterClubRequest request2 = new RegisterClubRequest { ClubName = "FC Barcelona" };
            var response2 = useCase.Execute(request2);            
            Assert.True(response2.WasRequestFulfilled && response2.ClubId.HasValue);
            Assert.True(response1.ClubId.Value != response2.ClubId.Value);
        }

        [Fact]
        public void LogoUrlRequiresScheme() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "google.com"};
            var response = useCase.Execute(request1);            
            Assert.False(response.WasRequestFulfilled);
        }
        
        [Fact]
        public void HttpLogoUrlIsSaved() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "http://google.com"};
            var response = useCase.Execute(request1);            
            Assert.True(response.WasRequestFulfilled && response.ClubId.HasValue);
        }

        [Fact]
        public void HttpsLogoUrlIsSaved() {
            RegisterClubRequest request1 = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "https://google.com"};
            var response = useCase.Execute(request1);            
            Assert.True(response.WasRequestFulfilled && response.ClubId.HasValue);
        }
    }
}
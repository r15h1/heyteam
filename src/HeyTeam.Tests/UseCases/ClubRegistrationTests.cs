using System;
using Xunit;
using HeyTeam.Core.Validation;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using HeyTeam.Lib.Data;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Tests.UseCases
{
    public class ClubRegistrationTests {
        private readonly IUseCase<RegisterClubRequest, Response<Guid?>> useCase;

        public ClubRegistrationTests() {   
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);         
            IValidator<RegisterClubRequest> validator = new RegisterClubRequestValidator();
            IClubRepository repository = new ClubRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            this.useCase = new RegisterClubUseCase(repository, validator);
        }

        [Fact]
        public void ClubNameAndUrlCannotBeNull() {
            RegisterClubRequest request = BuildRequest();
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 2);
        }

        [Fact]
        public void ClubNameCannotBeEmpty() {
            RegisterClubRequest request = BuildRequest(string.Empty, "http://manutd.com");
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void ClubNameCannotBeWhiteSpace() {
            RegisterClubRequest request = BuildRequest("  ", "http://manutd.com");
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void UrlCannotBeNull() {
            RegisterClubRequest request = BuildRequest("Manchester United", null);
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void UrlCannotBeEmpty() {
            RegisterClubRequest request = BuildRequest("Manchester United", string.Empty);
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void UrlCannotBeWhiteSpace() {
            RegisterClubRequest request = BuildRequest("Manchester United", "  ");
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void UrlCannotBeInvalid() {
            RegisterClubRequest request = BuildRequest("Manchester United", "abc");
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void UrlRequiresScheme() {
            RegisterClubRequest request1 = BuildRequest("Manchester United", "manutd.com");
            var response = useCase.Execute(request1);            
            Assert.False(response.WasRequestFulfilled);
        }

        [Fact]
        public void UrlMustBeUnique() {
            RegisterClubRequest request1 = BuildRequest("Manchester United", "http://manutd.com");
            var response1 = useCase.Execute(request1);            
            Assert.True(response1.WasRequestFulfilled && response1.Result.HasValue);

            RegisterClubRequest request2 = BuildRequest("FC Barcelona", "http://manutd.com");
            var response2 = useCase.Execute(request2);            
            Assert.True(!response2.WasRequestFulfilled && response2.Exception.GetType() == typeof(DuplicateEntryException));
        }

        [Fact]
        public void TwoClubWithValidNameshaveDifferentIds() {
            RegisterClubRequest request1 = BuildRequest("Manchester United", "http://manutd.com");
            var response1 = useCase.Execute(request1);            
            Assert.True(response1.WasRequestFulfilled && response1.Result.HasValue);

            RegisterClubRequest request2 = BuildRequest("FC Barcelona", "http://barca.com");
            var response2 = useCase.Execute(request2);            
            Assert.True(response2.WasRequestFulfilled && response2.Result.HasValue);
            Assert.True(response1.Result.Value != response2.Result.Value);
        }
        
        [Fact]
        public void ValidNameAndHttpUrlIsSaved() {
            RegisterClubRequest request1 = BuildRequest("Manchester United", "http://google.com");
            var response = useCase.Execute(request1);            
            Assert.True(response.WasRequestFulfilled && response.Result.HasValue);
        }

        [Fact]
        public void ValidNameAndHttpsUrlIsSaved() {
            var request1 = BuildRequest("Manchester United" , "https://manutd.com");
            var response = useCase.Execute(request1);            
            Assert.True(response.WasRequestFulfilled && response.Result.HasValue);
        }

        private RegisterClubRequest BuildRequest(string name = null, string url = null) => new RegisterClubRequest { Name = name , Url = url};
    }
}
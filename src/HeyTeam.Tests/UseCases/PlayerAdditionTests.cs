using System;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class PlayerAdditionTests {
        private readonly IUseCase<AddPlayerRequest, Response<Guid?>> useCase;
        private readonly Guid squadId;

        public PlayerAdditionTests() {
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);   
            IValidator<AddPlayerRequest> validator = new AddPlayerRequestValidator();
            var squadRepository = new SquadRepository(new ConnectionFactory(connectionString));
            var playerRepository = new PlayerRepository(new ConnectionFactory(connectionString));
            useCase = new AddPlayerUseCase(squadRepository, playerRepository, validator);
            squadId = SetupSquad(squadRepository, connectionString);
        }

        private Guid SetupSquad(ISquadRepository squadRepository, string connectionString)
        {
            var clubRepository = new ClubRepository(new ConnectionFactory(connectionString));
            var registerUseCase = new RegisterClubUseCase(clubRepository, new RegisterClubRequestValidator());
            RegisterClubRequest registerRequest = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "http://manutd.com"};
            var registerResponse = registerUseCase.Execute(registerRequest);  
            
            var addSquadRequest = new AddSquadRequest{ ClubId = registerResponse.Result.Value, SquadName = "U10" };
            var addSquadUseCase = new AddSquadUseCase(clubRepository, squadRepository, new AddSquadRequestValidator());
            var addSquadResponse = addSquadUseCase.Execute(addSquadRequest);

            return addSquadResponse.Result.Value;
        }

        [Fact]
        public void EmptyPlayerHas5ErrorMessages() {            
            var request = new AddPlayerRequest();
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 5);   
        }

        [Fact]
        public void PlayerSquadIdCannotBeEmpty() {            
            var request = new AddPlayerRequest() {
                FirstName = "John", LastName = "Smith", 
                DominantFoot = 'R', Nationality = "Canada" 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerFirstNameCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                LastName = "Smith", SquadId = squadId, 
                DominantFoot = 'R', Nationality = "Canada"
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerLastNameCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                FirstName = "John", SquadId = squadId, 
                DominantFoot = 'R', Nationality = "Canada" 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerDominantFootCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                SquadId = squadId, FirstName = "John", 
                LastName = "Smith", Nationality = "Canada" 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerNationalityCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                FirstName = "John", LastName = "Smith", 
                SquadId = squadId, DominantFoot = 'L'
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerDominantFootCanBeRightOrLeftOnly() {            
            var request = new AddPlayerRequest() { 
                FirstName = "John", LastName = "Smith", 
                SquadId = squadId, Nationality = "Canada", DominantFoot = 'C' 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   

            request.DominantFoot = 'L';
            response = useCase.Execute(request);
            Assert.True(response.WasRequestFulfilled);

            request.DominantFoot = 'R';
            response = useCase.Execute(request);
            Assert.True(response.WasRequestFulfilled);

        }
    }
}
using System;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class PlayerAdditionTests {
        private readonly IUseCase<AddPlayerRequest, Response<Guid?>> useCase;

        public PlayerAdditionTests() {
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);   
            IValidator<AddPlayerRequest> validator = new AddPlayerRequestValidator();            
            var squadRepository = new SquadRepository(new ConnectionFactory(connectionString));
            var playerRepository = new PlayerRepository(new ConnectionFactory(connectionString));
            useCase = new AddPlayerUseCase(squadRepository, playerRepository, validator);
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
                DominantFoot = "R", Nationality = "Canada" 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerFirstNameCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                LastName = "Smith", SquadId = Guid.NewGuid(), 
                DominantFoot = "R", Nationality = "Canada"
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerLastNameCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                FirstName = "John", SquadId = Guid.NewGuid(), 
                DominantFoot = "R", Nationality = "Canada" 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerDominantFootCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                SquadId = Guid.NewGuid(), FirstName = "John", 
                LastName = "Smith", Nationality = "Canada" 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerNationalityCannotBeEmpty() {            
            var request = new AddPlayerRequest() { 
                FirstName = "John", LastName = "Smith", 
                SquadId = Guid.NewGuid(), DominantFoot = "R" 
            };

            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }
    }
}
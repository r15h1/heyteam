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
        public void PlayerIdCannotBeEmpty() {            
            var request = new AddPlayerRequest();
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }
    }
}
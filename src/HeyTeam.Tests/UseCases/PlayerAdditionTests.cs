using System;
using System.Linq;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Data;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using HeyTeam.Util;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class PlayerAdditionTests {
        private readonly IUseCase<AddPlayerRequest, Response<Guid?>> useCase;
        private readonly IPlayerRepository playerRepository;
        private readonly Guid squadId;
        private readonly string connectionString;

        public PlayerAdditionTests() {
            connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);   
            IValidator<AddPlayerRequest> validator = new AddPlayerRequestValidator();
            var squadRepository = new SquadRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            playerRepository = new PlayerRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            useCase = new AddPlayerUseCase(squadRepository, playerRepository, validator);
            squadId = SetupSquad(squadRepository, connectionString);
        }

        private Guid SetupSquad(ISquadRepository squadRepository, string connectionString)
        {
            var clubRepository = new ClubRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            var registerUseCase = new RegisterClubUseCase(clubRepository, new RegisterClubRequestValidator());
            RegisterClubRequest registerRequest = new RegisterClubRequest { Name = "Manchester United" , Url = "http://manutd.com"};
            var registerResponse = registerUseCase.Execute(registerRequest);  
            
            var addSquadRequest = new AddSquadRequest{ ClubId = registerResponse.Result.Value, SquadName = "U10" };
            var addSquadUseCase = new AddSquadUseCase(clubRepository, squadRepository, new AddSquadRequestValidator());
            var addSquadResponse = addSquadUseCase.Execute(addSquadRequest);

            return addSquadResponse.Result.Value;
        }

        private AddPlayerRequest BuildAddRequest()
        {
            return new AddPlayerRequest()
            {
                FirstName = "John",
                LastName = "Smith",
                SquadId = squadId,
                DominantFoot = 'R',
                Nationality = "Canada",
                DateOfBirth = DateTime.Now.AddYears(-13),
                Email = "john.smith@bigmail.com"
            };
        }

        [Fact]
        public void EmptyPlayerHas6ErrorMessages() {            
            var request = new AddPlayerRequest();
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 7);   
        }

        [Fact]
        public void PlayerSquadIdCannotBeEmpty() {            
            var request = BuildAddRequest();
            request.SquadId = Guid.Empty;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerFirstNameCannotBeNull()
        {
            var request = BuildAddRequest();
            request.FirstName = null;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void PlayerFirstNameCannotBeEmpty()
        {
            var request = BuildAddRequest();
            request.FirstName = string.Empty;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void PlayerFirstNameCannotBeWhiteSpace()
        {
            var request = BuildAddRequest();
            request.FirstName = "  ";
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);
        }

        [Fact]
        public void PlayerLastNameCannotBeNull() {            
            var request = BuildAddRequest();
            request.LastName = null;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerLastNameCannotBeEmpty() {            
            var request = BuildAddRequest();
            request.LastName = string.Empty;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerLastNameCannotBeWhiteSpace() {            
            var request = BuildAddRequest();
            request.LastName = "  ";
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerEmailCannotBeNull() {            
            var request = BuildAddRequest();
            request.LastName = null;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerEmailCannotBeEmpty() {            
            var request = BuildAddRequest();
            request.LastName = "";
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerEmailCannotBeWhitespace() {            
            var request = BuildAddRequest();
            request.LastName = "  ";
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerEmailCannotBeInvalid() {            
            var request = BuildAddRequest();
            request.Email = "joe@";
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }        

        [Fact]
        public void PlayerDominantFootCannotBeEmpty() {            
            var request = BuildAddRequest();
            request.DominantFoot = char.MinValue;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerNationalityCannotBeNull() {            
            var request = BuildAddRequest();
            request.Nationality = null;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerNationalityCannotBeEmpty() {            
            var request = BuildAddRequest();
            request.Nationality = string.Empty;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerNationalityCannotBeWhiteSpace() {            
            var request = BuildAddRequest();
            request.Nationality = "  ";
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerDominantFootCanBeRightOrLeftOnly() {            
            //no 'R' or 'L' in this string
            var testFoot = " abcdefghijkmnopqstuvwxyzABCDEFGHIJKMNOPQSTUVWXYZ&^%$#@&*()&12345678900/*-+|\\][";
            var request = BuildAddRequest();
            var success = true;
            Response<Guid?> response = null;
            foreach(var c in testFoot) {
                request.DominantFoot = c;
                response = useCase.Execute(request);
                if (response.WasRequestFulfilled || response.Errors.Count == 0) {
                    success = false;
                    break;
                }                
            }

            Assert.True(success);
        }

        [Fact]
        public void PlayerDateOfBirthCannotBeEmpty() {
            var request = BuildAddRequest();
            request.DateOfBirth = null;
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerDateOfBirthCannotBeInTheFuture() {
            var request = BuildAddRequest();
            request.DateOfBirth = DateTime.Now.AddDays(1);
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);   
        }

        [Fact]
        public void PlayerCannotBeAddedToInexistentSquad() {
            var request = BuildAddRequest();
            request.SquadId = Guid.NewGuid();
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Exception.GetType() == typeof(SquadNotFoundException));   
        }

        [Fact]
        public void PlayerCanBeAddedToValidSquad() {
            var request = BuildAddRequest();
            var response = useCase.Execute(request);
            Assert.True(response.WasRequestFulfilled && !response.Result.Value.IsEmpty());   
        }

        [Fact]
        public void CheckingNewPlayerValues() {
            var request = BuildAddRequest();
            var response = useCase.Execute(request);
            var player = playerRepository.Get(response.Result.Value);

            Assert.True(player.DateOfBirth == request.DateOfBirth.Value);
            Assert.True(player.DominantFoot == request.DominantFoot);
            Assert.True(player.FirstName.Equals(request.FirstName));
            Assert.True(player.LastName.Equals(request.LastName));
            Assert.True(player.Nationality.Equals(request.Nationality));
            Assert.True(player.SquadId == request.SquadId);
            Assert.True(player.SquadNumber == request.SquadNumber);
            Assert.True(player.Email.Equals(request.Email));            
            //missing positions
        }        
    }
}
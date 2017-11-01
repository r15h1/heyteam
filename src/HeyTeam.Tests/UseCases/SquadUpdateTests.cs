using System;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Data;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class SquadUpdateTests {
        private readonly ISquadRepository squadRepository;
        private IUseCase<UpdateSquadRequest, Response<Guid?>> updateSquadUseCase;
        private IUseCase<AddSquadRequest, Response<Guid?>> addSquadUseCase;
        private readonly Guid manUtdClubId, barcaClubId;

        public SquadUpdateTests() {
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);

            IValidator<UpdateSquadRequest> validator = new UpdateSquadRequestValidator();
            IClubRepository clubRepository = new ClubRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            this.squadRepository = new SquadRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            this.updateSquadUseCase = new UpdateSquadUseCase(clubRepository, squadRepository, validator);

            this.addSquadUseCase = new AddSquadUseCase(clubRepository, squadRepository, new AddSquadRequestValidator());

            var registerUseCase = new RegisterClubUseCase(clubRepository, new RegisterClubRequestValidator());
            RegisterClubRequest registerRequest = new RegisterClubRequest { Name = "Manchester United" , Url = "http://manutd.com"};
            var registerResponse = registerUseCase.Execute(registerRequest);  
            this.manUtdClubId = registerResponse.Result.Value;
            
            registerRequest = new RegisterClubRequest { Name = "Barcelona" , Url = "http://barca.com"};
            registerResponse = registerUseCase.Execute(registerRequest);  
            this.barcaClubId = registerResponse.Result.Value;
        }

        [Fact]
        public void ClubIdCannotBeEmpty() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.Empty, SquadId = Guid.NewGuid(), SquadName = "U10" };
            var response = updateSquadUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);            
        }

        [Fact]
        public void SquadIdCannotBeEmpty() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.Empty, SquadName = "U10" };
            var response = updateSquadUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);            
        }

        [Fact]
        public void SquadNameCannotBeNull() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.NewGuid(), SquadName = null };
            var response = updateSquadUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);            
        }

        [Fact]
        public void SquadNameCannotBeEmpty() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.NewGuid(), SquadName = "" };
            var response = updateSquadUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);            
        }

        [Fact]
        public void SquadNameCannotBeWhitespace() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.NewGuid(), SquadName = " " };
            var response = updateSquadUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);            
        }

        [Fact]
        public void ClubMustExist() {
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.NewGuid(), SquadName = "U10" };
            var response = updateSquadUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Exception.GetType() == typeof(ClubNotFoundException));            
        }

        [Fact]
        public void SquadMustExist() {
            var request = new UpdateSquadRequest{ ClubId = manUtdClubId, SquadId = Guid.NewGuid(), SquadName = "U10" };
            var response = updateSquadUseCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Exception.GetType() == typeof(SquadNotFoundException));            
        }

        [Fact]
        public void SquadNameInSameClubCannotBeDuplicate() {            
            var addRequest = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var addResponse = addSquadUseCase.Execute(addRequest);
            Assert.True(addResponse.WasRequestFulfilled);
            Assert.True(addResponse.Result.HasValue && addResponse.Result.Value != Guid.Empty);
            Guid u10id = addResponse.Result.Value;

            addRequest = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U11" };
            addResponse = addSquadUseCase.Execute(addRequest);
            Assert.True(addResponse.WasRequestFulfilled);
            Assert.True(addResponse.Result.HasValue && addResponse.Result.Value != Guid.Empty);

            var updateRequest = new UpdateSquadRequest{ ClubId = manUtdClubId, SquadId = u10id, SquadName = "U11" };
            var response = updateSquadUseCase.Execute(updateRequest);
            Assert.True(!response.WasRequestFulfilled && response.Exception.GetType() == typeof(DuplicateEntryException));            
        }

        [Fact]
        public void UniqueSquadNameInSameClubIsSaved() {            
            var addRequest = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var addResponse = addSquadUseCase.Execute(addRequest);
            Assert.True(addResponse.WasRequestFulfilled);
            Assert.True(addResponse.Result.HasValue && addResponse.Result.Value != Guid.Empty);
            Guid u10id = addResponse.Result.Value;

            addRequest = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U11" };
            addResponse = addSquadUseCase.Execute(addRequest);
            Assert.True(addResponse.WasRequestFulfilled);
            Assert.True(addResponse.Result.HasValue && addResponse.Result.Value != Guid.Empty);

            var u10 = squadRepository.GetSquad(u10id);
            Assert.True(u10.Name.Equals("U10"));

            var updateRequest = new UpdateSquadRequest{ ClubId = manUtdClubId, SquadId = u10id, SquadName = "U12" };
            var response = updateSquadUseCase.Execute(updateRequest);            
            Assert.True(response.WasRequestFulfilled);

            u10 = squadRepository.GetSquad(u10id);
            Assert.True(u10.Name.Equals("U12"));
        }
    }
}
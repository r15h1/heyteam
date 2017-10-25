using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Entities;
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
    public class SquadAdditionTests {
        private IUseCase<AddSquadRequest, Response<Guid?>> useCase;
        private readonly Guid manUtdClubId, barcaClubId;
        private readonly IClubRepository clubRepository;
        private readonly ISquadRepository squadRepository;

        public SquadAdditionTests() {    
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);         
                        
            IValidator<AddSquadRequest> validator = new AddSquadRequestValidator();
            clubRepository = new ClubRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            squadRepository = new SquadRepository(new Data.ConnectionFactory(new DatabaseSettings { ConnectionString = connectionString } ));
            this.useCase = new AddSquadUseCase(clubRepository, squadRepository, validator);

            var registerUseCase = new RegisterClubUseCase(clubRepository, new RegisterClubRequestValidator());
            RegisterClubRequest registerRequest = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "http://manutd.com"};
            var registerResponse = registerUseCase.Execute(registerRequest);  
            this.manUtdClubId = registerResponse.Result.Value;
            
            registerRequest = new RegisterClubRequest { ClubName = "Barcelona" , ClubLogoUrl = "http://barca.com"};
            registerResponse = registerUseCase.Execute(registerRequest);  
            this.barcaClubId = registerResponse.Result.Value;
        }

        [Fact]
        public void ClubIdCannotBeEmpty() {            
            var request = new AddSquadRequest{ ClubId = Guid.Empty, SquadName = "U10" };
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);            
        }

        [Fact]
        public void ClubMustExist() {            
            var request = new AddSquadRequest{ ClubId = Guid.NewGuid(), SquadName = "U10" };    
            var response = useCase.Execute(request);            
            Assert.True(!response.WasRequestFulfilled && response.Exception.GetType() == typeof(ClubNotFoundException));
        }

        [Fact]
        public void SquadNameCannotBeNull() {            
            var request = new AddSquadRequest{ ClubId = Guid.NewGuid(), SquadName = null };            
            var response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Errors.Count == 1);  
        }

        
        [Fact]
        public void SquadNameInSameClubCannotBeDuplicate() {
            var request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var response = useCase.Execute(request);
            Assert.True(response.WasRequestFulfilled);
            Assert.True(response.Result.HasValue && response.Result.Value != Guid.Empty);

            request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            response = useCase.Execute(request);
            Assert.True(!response.WasRequestFulfilled && response.Exception.GetType() == typeof(DuplicateEntryException));
        }

        [Fact]
        public void CanAddMultipleSquadsWithDifferentNamesToSameClub() {
            var request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var response1 = useCase.Execute(request);
            Assert.True(response1.WasRequestFulfilled);
            Assert.True(response1.Result.HasValue && response1.Result.Value != Guid.Empty);
            
            request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U11" };
            var response2 = useCase.Execute(request);
            Assert.True(response2.WasRequestFulfilled);
            Assert.True(response2.Result.HasValue && response2.Result.Value != Guid.Empty);;

            request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U12" };
            var response3 = useCase.Execute(request);
            Assert.True(response3.WasRequestFulfilled);
            Assert.True(response3.Result.HasValue && response3.Result.Value != Guid.Empty);

            Assert.True(response1.Result != response2.Result && response1.Result != response3.Result && response2.Result != response3.Result);

            var clubs = clubRepository.Get(manUtdClubId);
            Assert.True(clubs.Squads.Count == 3);
        }

        [Fact]
        public void CanAddSquadWithSameNameToDifferentClub() {
            var manutd = clubRepository.Get(manUtdClubId);
            var barca = clubRepository.Get(barcaClubId);
            Assert.True(manutd.Squads.Count == 0 && barca.Squads.Count == 0);

            var request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var response1 = useCase.Execute(request);
            Assert.True(response1.WasRequestFulfilled);
            Assert.True(response1.Result.HasValue && response1.Result.Value != Guid.Empty);

            request = new AddSquadRequest{ ClubId = barcaClubId, SquadName = "U10" };
            var response2 = useCase.Execute(request);
            Assert.True(response2.WasRequestFulfilled);
            Assert.True(response2.Result.HasValue && response2.Result.Value != Guid.Empty);

            Assert.True(response1.Result != response2.Result);

            manutd = clubRepository.Get(manUtdClubId);
            barca = clubRepository.Get(barcaClubId);

            Assert.True(manutd.Squads.Count == 1 && barca.Squads.Count == 1);
        }        
    }
}
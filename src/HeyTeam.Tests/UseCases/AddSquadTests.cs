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
    public class AddSquadTests {
        private IUseCase<AddSquadRequest, AddSquadResponse> useCase;
        private readonly Guid manUtdClubId, barcaClubId;

        public AddSquadTests() {    
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);         
                        
            IValidator<AddSquadRequest> validator = new AddSquadRequestValidator();
            IClubRepository clubRepository = new ClubRepository(new ConnectionFactory(connectionString));
            ISquadRepository squadRepository = new SquadRepository(new ConnectionFactory(connectionString));
            this.useCase = new AddSquadUseCase(clubRepository, squadRepository, validator);

            var registerUseCase = new RegisterClubUseCase(clubRepository, new RegisterClubRequestValidator());
            RegisterClubRequest registerRequest = new RegisterClubRequest { ClubName = "Manchester United" , ClubLogoUrl = "http://manutd.com"};
            var registerResponse = registerUseCase.Execute(registerRequest);  
            this.manUtdClubId = registerResponse.ClubId.Value;
            
             registerRequest = new RegisterClubRequest { ClubName = "Barcelona" , ClubLogoUrl = "http://barca.com"};
            registerResponse = registerUseCase.Execute(registerRequest);  
            this.barcaClubId = registerResponse.ClubId.Value;
        }

        [Fact]
        public void ClubIdCannotBeEmpty()
        {            
            var request = new AddSquadRequest{ ClubId = Guid.Empty, SquadName = "U10" };
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);            
        }

        [Fact]
        public void SquadsCannotBeAddedToInexistentClubs()
        {            
            var request = new AddSquadRequest{ ClubId = Guid.NewGuid(), SquadName = "U10" };              
            Assert.Throws<ClubNotFoundException>(() =>  useCase.Execute(request));
        }

        [Fact]
        public void SquadNameCannotBeNull()
        {            
            var request = new AddSquadRequest{ ClubId = Guid.NewGuid(), SquadName = null };            
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);  
        }

        
        [Fact]
        public void SquadNameInSameClubCannotBeDuplicate()
        {
            var request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var response = useCase.Execute(request);
            Assert.True(response.ValidationResult.IsValid);
            Assert.True(response.SquadId.HasValue && response.SquadId.Value != Guid.Empty);

            request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            Assert.Throws<DuplicateEntryException>(() => useCase.Execute(request));
        }

        [Fact]
        public void CanAddMultipleSquadsWithDifferentNamesToSameClub()
        {
            var request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var response1 = useCase.Execute(request);
            Assert.True(response1.ValidationResult.IsValid);
            Assert.True(response1.SquadId.HasValue && response1.SquadId.Value != Guid.Empty);
            
            request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U11" };
            var response2 = useCase.Execute(request);
            Assert.True(response2.ValidationResult.IsValid);
            Assert.True(response2.SquadId.HasValue && response2.SquadId.Value != Guid.Empty);;

            request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U12" };
            var response3 = useCase.Execute(request);
            Assert.True(response3.ValidationResult.IsValid);
            Assert.True(response3.SquadId.HasValue && response3.SquadId.Value != Guid.Empty);

            Assert.True(response1.SquadId != response2.SquadId && response1.SquadId != response3.SquadId && response2.SquadId != response3.SquadId);
        }

        [Fact]
        public void CanAddSquadWithSameNameToDifferentClub()
        {
            var request = new AddSquadRequest{ ClubId = manUtdClubId, SquadName = "U10" };
            var response1 = useCase.Execute(request);
            Assert.True(response1.ValidationResult.IsValid);
            Assert.True(response1.SquadId.HasValue && response1.SquadId.Value != Guid.Empty);

            request = new AddSquadRequest{ ClubId = barcaClubId, SquadName = "U10" };
            var response2 = useCase.Execute(request);
            Assert.True(response2.ValidationResult.IsValid);
            Assert.True(response2.SquadId.HasValue && response2.SquadId.Value != Guid.Empty);

            Assert.True(response1.SquadId != response2.SquadId);
        }        
    }
}
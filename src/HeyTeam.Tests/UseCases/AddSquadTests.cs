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
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class AddSquadTests {
        private IUseCase<AddSquadRequest, AddSquadResponse> useCase;
        private IValidator<AddSquadRequest> validator;
        public AddSquadTests() {                    
            validator = new AddSquadRequestValidator();
            //useCase = new AddSquadUseCase(fixture.ClubRepository, squadRepository, validator);
        }

        [Fact]
        public void ClubIdCannotBeEmpty()
        {
            var options = GetContextOptions("ClubIdCannotBeEmpty");
            var request = new AddSquadRequest{ ClubId = Guid.Empty, SquadName = "U10" };
            using (var context = new ClubContext(options))
            {
                IClubRepository clubRepository = new ClubRepository(context);
                ISquadRepository squadRepository = new SquadRepository(context);
                useCase = new AddSquadUseCase(clubRepository, squadRepository, validator);
                var response = useCase.Execute(request);
                Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);
            }
        }

        [Fact]
        public void SquadsCannotBeAddedToInexistentClubs()
        {
            var options = GetContextOptions("SquadsCannotBeAddedToInexistentClubs");
            var request = new AddSquadRequest{ ClubId = Guid.NewGuid(), SquadName = "U10" };  
            using (var context = new ClubContext(options))
            {
                IClubRepository clubRepository = new ClubRepository(context);
                ISquadRepository squadRepository = new SquadRepository(context);
                useCase = new AddSquadUseCase(clubRepository, squadRepository, validator);          
                Assert.Throws<ClubNotFoundException>(() =>  useCase.Execute(request));
            }
        }

        [Fact]
        public void SquadNameCannotBeNull()
        {
            var options = GetContextOptions("SquadNameCannotBeNull");
            var request = new AddSquadRequest{ ClubId = Guid.NewGuid(), SquadName = null };
            using (var context = new ClubContext(options))
            {
                IClubRepository clubRepository = new ClubRepository(context);
                ISquadRepository squadRepository = new SquadRepository(context);
                useCase = new AddSquadUseCase(clubRepository, squadRepository, validator);
                useCase.Execute(request);
            }



            // var request = new AddSquadRequest{ ClubId = fixture.Manutd.Id, SquadName = null };
            // var response = useCase.Execute(request);
            // Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);
        }

        // [Fact]
        // public void SquadNameInSameClubCannotBeDuplicate()
        // {
        //     var request = new AddSquadRequest{ ClubId = fixture.Manutd.Id, SquadName = "U10" };
        //     var response = useCase.Execute(request);
        //     Assert.True(response.ValidationResult.IsValid);
        //     Assert.True(response.SquadId.HasValue && response.SquadId.Value != Guid.Empty);

        //     request = new AddSquadRequest{ ClubId = fixture.Manutd.Id, SquadName = "U10" };
        //     Assert.Throws<DuplicateEntryException>(() => useCase.Execute(request));
        // }

        // [Fact]
        // public void CanAddMultipleSquadsWithDifferentNamesToSameClub()
        // {
        //     var request = new AddSquadRequest{ ClubId = fixture.Manutd.Id, SquadName = "U11" };
        //     var response1 = useCase.Execute(request);
        //     Assert.True(response1.ValidationResult.IsValid);
            
        //     request = new AddSquadRequest{ ClubId = fixture.Manutd.Id, SquadName = "U12" };
        //     var response2 = useCase.Execute(request);
        //     Assert.True(response2.ValidationResult.IsValid);

        //     request = new AddSquadRequest{ ClubId = fixture.Manutd.Id, SquadName = "U13" };
        //     var response3 = useCase.Execute(request);
        //     Assert.True(response3.ValidationResult.IsValid);

        //     Assert.True(response1.SquadId != response2.SquadId && response1.SquadId != response3.SquadId && response2.SquadId != response3.SquadId);
        // }

        // [Fact]
        // public void CanAddSquadWithSameNameToDifferentClub()
        // {
        //     var request = new AddSquadRequest{ ClubId = fixture.Barca.Id, SquadName = "U11" };
        //     var response = useCase.Execute(request);
        //     Assert.True(response.ValidationResult.IsValid);            
        // }

        private DbContextOptions<ClubContext> GetContextOptions(string db) => new DbContextOptionsBuilder<ClubContext>().UseInMemoryDatabase(databaseName: db).Options;
    }
}
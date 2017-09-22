using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Fixtures;
using HeyTeam.Tests.Repositories;
using Xunit;

namespace HeyTeam.Tests.UseCases {

    [Collection("ClubFixtureCollection")]
    public class AddSquadTests {
        private readonly IUseCase<AddSquadRequest, AddSquadResponse> useCase;
        private readonly ISquadRepository squadRepository;
        private readonly List<Club> clubs;

        public AddSquadTests(ClubFixture fixture) {
            clubs = fixture.Clubs; 
            squadRepository = fixture.SquadRepository;             
            IValidator<AddSquadRequest> validator = new AddSquadRequestValidator();
            useCase = new AddSquadUseCase(fixture.ClubRepository, squadRepository, validator);
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
            var request = new AddSquadRequest{ ClubId = clubs.FirstOrDefault().Id, SquadName = null };
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);
        }

        [Fact]
        public void SquadNameInSameClubCannotBeDuplicate()
        {
            var request = new AddSquadRequest{ ClubId = clubs.FirstOrDefault().Id, SquadName = "U10" };
            var response = useCase.Execute(request);
            Assert.True(response.ValidationResult.IsValid);
            Assert.True(response.SquadId.HasValue && response.SquadId.Value != Guid.Empty);

            request = new AddSquadRequest{ ClubId = clubs.FirstOrDefault().Id, SquadName = "U10" };
            Assert.Throws<DuplicateEntryException>(() => useCase.Execute(request));
        }
    }
}
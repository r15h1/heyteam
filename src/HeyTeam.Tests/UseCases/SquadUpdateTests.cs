using System;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using Xunit;

namespace HeyTeam.Tests.UseCases {
    public class SquadUpdateTests {        
        private IUseCase<UpdateSquadRequest, UpdateSquadResponse> useCase;
        private readonly Guid manUtdClubId, barcaClubId;

        public SquadUpdateTests() {
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString);

            IValidator<UpdateSquadRequest> validator = new UpdateSquadRequestValidator();
            IClubRepository clubRepository = new ClubRepository(new ConnectionFactory(connectionString));
            ISquadRepository squadRepository = new SquadRepository(new ConnectionFactory(connectionString));
            this.useCase = new UpdateSquadUseCase(clubRepository, squadRepository, validator);
        }

        [Fact]
        public void ClubIdCannotBeEmpty() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.Empty, SquadId = Guid.NewGuid(), SquadName = "U10" };
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);            
        }

        [Fact]
        public void SquadIdCannotBeEmpty() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.Empty, SquadName = "U10" };
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);            
        }

        [Fact]
        public void SquadNameCannotBeNull() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.NewGuid(), SquadName = null };
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);            
        }

        [Fact]
        public void SquadNameCannotBeEmpty() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.NewGuid(), SquadName = "" };
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);            
        }

        [Fact]
        public void SquadNameCannotBeWhitespace() {            
            var request = new UpdateSquadRequest{ ClubId = Guid.NewGuid(), SquadId = Guid.NewGuid(), SquadName = " " };
            var response = useCase.Execute(request);
            Assert.True(!response.ValidationResult.IsValid && response.ValidationResult.Messages.Count == 1);            
        }
    }
}
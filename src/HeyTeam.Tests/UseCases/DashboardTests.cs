using System;
using System.Collections.Generic;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Dashboard;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using Xunit;

namespace HeyTeam.Tests.UseCases
{
    public class DashboardTests {
        private readonly IUseCase<DashboardRequest, Response<List<Group>>> useCase;
        private readonly ClubRepository clubRepository;
        private readonly SquadRepository squadRepository;
        private readonly IDashboardRepository dashboardRepository;
        private readonly IIdentityManager identityManager;
        private readonly string connectionString;
        private Club club;

        public DashboardTests() {
            this.connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString); 
            IValidator<DashboardRequest> validator = new DashboardRequestValidator();
            this.clubRepository = new ClubRepository(new ConnectionFactory(connectionString));   
            this.squadRepository = new SquadRepository(new ConnectionFactory(connectionString));         
            dashboardRepository = new DashboardRepository(new ConnectionFactory(connectionString));            
            AbstractDashboardBuilder dashboardBuilder = new DashboardBuilder(dashboardRepository, identityManager);
            useCase = new DashboardUseCase(clubRepository, dashboardBuilder, validator);            
            identityManager = Util.GetIdentityInstance(connectionString);

            var clubId = SetupClub();
            SetUpSquads(clubId);
            club = clubRepository.Get(clubId);

        }

        private void SetUpSquads(Guid clubId)
        {
            var validator =  new AddSquadRequestValidator();
            var useCase = new AddSquadUseCase(clubRepository, squadRepository, validator);
            useCase.Execute(new AddSquadRequest{ ClubId = clubId, SquadName = "U10" });
            useCase.Execute(new AddSquadRequest{ ClubId = clubId, SquadName = "U11" });
            useCase.Execute(new AddSquadRequest{ ClubId = clubId, SquadName = "U12" });
        }

        private Guid SetupClub()
        {
            var validator = new RegisterClubRequestValidator();
            var useCase = new RegisterClubUseCase(clubRepository, validator);
            var clubRequest = new RegisterClubRequest { ClubName = "Lions FC", ClubLogoUrl = "http://lionsfc.com/logo.jpg" };
            var clubResponse = useCase.Execute(clubRequest);
            return clubResponse.Result.Value;
        }

        [Fact]
        public void Test1() {
            //var result = await identityManager.CreateUser("joe@gmail.com", "Abc225#$kLASk");
            Assert.True(1 == 1);
        }
    }
}
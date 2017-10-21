using System;
using System.Collections.Generic;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
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
        private readonly IDashboardRepository dashboardRepository;
        private readonly IIdentityManager identityManager;

        public DashboardTests() {
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString); 
            IValidator<DashboardRequest> validator = new DashboardRequestValidator();
            var clubRepository = new ClubRepository(new ConnectionFactory(connectionString));            
            dashboardRepository = new DashboardRepository(new ConnectionFactory(connectionString));            
            AbstractDashboardBuilder dashboardBuilder = new DashboardBuilder(dashboardRepository, identityManager);
            useCase = new DashboardUseCase(clubRepository, dashboardBuilder, validator);            
            identityManager = Util.GetIdentityInstance(connectionString);

        }

        [Fact]
        public async void Test1() {
            //var result = await identityManager.CreateUser("joe@gmail.com", "Abc225#$kLASk");
            Assert.True(1 == 1);
        }
    }
}
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
using HeyTeam.Identity;

namespace HeyTeam.Tests.UseCases {
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
            identityManager = new IdentityManager();
            AbstractDashboardBuilder dashboardBuilder = new DashboardBuilder(dashboardRepository, identityManager);
            useCase = new DashboardUseCase(clubRepository, dashboardBuilder, validator);
        }
    }
}
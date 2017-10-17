using System;
using System.Collections.Generic;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Dashboard;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;

namespace HeyTeam.Tests.UseCases {
    public class DashboardTests {
        private readonly IUseCase<DashboardRequest, Response<List<Group>>> useCase;
        private readonly IDashboardRepository dashboardRepository;
        private readonly IUserRepository userRepository;

        public DashboardTests() {
            string connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString); 
            IValidator<DashboardRequest> validator = new DashboardRequestValidator();
            var clubRepository = new ClubRepository(new ConnectionFactory(connectionString));            
            dashboardRepository = new DashboardRepository(new ConnectionFactory(connectionString));
            userRepository = new UserRepository(new ConnectionFactory(connectionString));
            AbstractDashboardBuilder dashboardBuilder = new DashboardBuilder(dashboardRepository, userRepository);
            useCase = new DashboardUseCase(clubRepository, dashboardBuilder, validator);
        }
    }
}
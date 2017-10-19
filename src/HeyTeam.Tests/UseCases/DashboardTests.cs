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
using Microsoft.AspNetCore.Identity;
using HeyTeam.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.EntityFrameworkCore;
using Xunit;

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
            AbstractDashboardBuilder dashboardBuilder = new DashboardBuilder(dashboardRepository, identityManager);
            useCase = new DashboardUseCase(clubRepository, dashboardBuilder, validator);

            var services = new ServiceCollection();
            services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            
            var context = new DefaultHttpContext();
            context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
            services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor { HttpContext = context });
            var serviceProvider = services.BuildServiceProvider();
            var Context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            identityManager = new IdentityManager(userManager);

        }

        [Fact]
        public async void Test1() {
            //var result = await identityManager.CreateUser("joe@gmail.com", "Abc225#$kLASk");
            Assert.True(1 == 1);
        }
    }
}
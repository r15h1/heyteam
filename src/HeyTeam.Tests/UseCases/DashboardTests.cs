using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Validation;
using HeyTeam.Tests.Data;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace HeyTeam.Tests.UseCases
{
    public class DashboardTests {
        private readonly IUseCase<DashboardRequest, Response<List<Group>>> useCase;
        private readonly ClubRepository clubRepository;
        private readonly SquadRepository squadRepository;
        private readonly IDashboardRepository dashboardRepository;
        private readonly IIdentityManager identityManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly string connectionString;
        private Club club;
        private readonly string playerUser = "email2@heyteam.com", adminUser = "admin@heyteam.com", coachUser = "coach@heyteam.com";

        public DashboardTests() {
            this.connectionString = $"Data Source=file:{Guid.NewGuid().ToString()}.sqlite";
            Database.Create(connectionString); 
            IValidator<DashboardRequest> validator = new DashboardRequestValidator();
            this.clubRepository = new ClubRepository(new ConnectionFactory(connectionString));   
            this.squadRepository = new SquadRepository(new ConnectionFactory(connectionString));  
            this.dashboardRepository = new DashboardRepository(new ConnectionFactory(connectionString));

            identityManager = Util.GetIdentityInstance(connectionString);
            roleManager = Util.GetRoleInstance(connectionString);          
            useCase = new DashboardUseCase(clubRepository, dashboardRepository, identityManager, validator);

            var clubId = SetupClub();
            SetUpSquads(clubId);
            club = clubRepository.Get(clubId);
            SetUpPlayers();
            SetupUsers();
        }

        private Guid SetupClub() {
            var validator = new RegisterClubRequestValidator();
            var clubUseCase = new RegisterClubUseCase(clubRepository, validator);
            var clubRequest = new RegisterClubRequest { ClubName = "Lions FC", ClubLogoUrl = "http://lionsfc.com/logo.jpg" };
            var clubResponse = clubUseCase.Execute(clubRequest);
            return clubResponse.Result.Value;
        }

        private void SetUpSquads(Guid clubId) {
            var validator =  new AddSquadRequestValidator();
            var squadUseCase = new AddSquadUseCase(clubRepository, squadRepository, validator);
            squadUseCase.Execute(new AddSquadRequest{ ClubId = clubId, SquadName = "U10" });
            squadUseCase.Execute(new AddSquadRequest{ ClubId = clubId, SquadName = "U11" });
        }

        private void SetUpPlayers() {      
            int j = 0;      
            foreach(var squad in club.Squads) {
                for(int i=0; i<3; i++)  {
                    AddPlayer(squad, i+j);
                    j++;
                }
            }
        }

        private void SetupUsers() {
            Task.WaitAll(
                new Task[] {
                    roleManager.CreateAsync(new IdentityRole("administrator")),
                    roleManager.CreateAsync(new IdentityRole("player")),
                    roleManager.CreateAsync(new IdentityRole("coach"))
            });
        

            Task.WaitAll ( new Task[] {
                identityManager.SetupUser( new Credential {
                    Email = adminUser,
                    Password = "$123abcD.%FFF",
                    Roles = new List<Roles> { Roles.Administrator }
                }),

                identityManager.SetupUser (
                    new Credential {
                        Email = playerUser,
                        Password = "$123abcD.%FFF",
                        Roles = new List<Roles> { Roles.Player }
                    }
                ),

                identityManager.SetupUser (
                    new Credential {
                        Email = coachUser,
                        Password = "$123abcD.%FFF",
                        Roles = new List<Roles> { Roles.Coach }
                    }
                )
            });
         }

        private void AddPlayer(Squad squad, int playerNumber) {
            var request = new AddPlayerRequest()
            {
                FirstName = $"F Name {playerNumber}",
                LastName =  $"L Name {playerNumber}",
                SquadId = squad.Guid,
                DominantFoot = 'R',
                Nationality = "Canada",
                DateOfBirth = DateTime.Now.AddYears(-13),
                Email = $"email{playerNumber}@heyteam.com"
            };

            var playerUseCase = new AddPlayerUseCase(squadRepository, new PlayerRepository(new ConnectionFactory(connectionString)), new AddPlayerRequestValidator());
            playerUseCase.Execute(request);
        }

        [Fact]
        public void AdminDashboardMustContainSquadSummary() {
            var dashboard = useCase.Execute(new DashboardRequest { Email = adminUser, ClubId = club.Guid }).Result;
            var squadGroup = dashboard.Where(i => i.Name.ToLowerInvariant().Equals("squads")).ToList();
            Assert.True(squadGroup.Count() == 1);
            var squads = squadGroup.FirstOrDefault().Items;
            Assert.True(squads.Count == 2);
        }

        [Fact]
        public void CoachDashboardMustNotContainSquadSummary() {
            var dashboard = useCase.Execute(new DashboardRequest { Email = coachUser, ClubId = club.Guid }).Result;
            var squadGroup = dashboard.Where(i => i.Name.ToLowerInvariant().Equals("squads")).ToList();
            Assert.True(squadGroup.Count() == 0);            
        }

        [Fact]
        public void PlayerDashboardMustNotContainSquadSummary() {
            var dashboard = useCase.Execute(new DashboardRequest { Email = playerUser, ClubId = club.Guid }).Result;
            var squadGroup = dashboard.Where(i => i.Name.ToLowerInvariant().Equals("squads")).ToList();
            Assert.True(squadGroup.Count() == 0);
        }
    }
}
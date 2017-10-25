using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases
{
    public class DashboardUseCase : IUseCase<DashboardRequest, Response<List<Dashboard.Group>>> {
        private readonly IClubRepository clubRepository;
        private readonly IDashboardRepository dashboardRepository;
        private readonly IIdentityManager identityManager;
        private readonly IValidator<DashboardRequest> validator;

        public DashboardUseCase(IClubRepository clubRepository, IDashboardRepository dashboardRepository, IIdentityManager identityManager, IValidator<DashboardRequest> validator) {
            this.clubRepository = clubRepository;
            this.dashboardRepository = dashboardRepository;
            this.identityManager = identityManager;
            this.validator = validator;
        }

        public Response<List<Dashboard.Group>> Execute(DashboardRequest request) {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<List<Dashboard.Group>>.CreateResponse(validationResult.Messages);
            
            var club = clubRepository.Get(request.ClubId);
            if (club == null)
                return Response<List<Dashboard.Group>>.CreateResponse(new ClubNotFoundException());

            return new Response<List<Dashboard.Group>>(BuildDashboard(request.ClubId, request.Email));            
        }

        private List<Group> BuildDashboard(Guid clubId, string email) {
            var roles = identityManager.GetRoles(email);
            var dashboard = new List<Group>();
            
            if(roles.Contains(Roles.Administrator))
                dashboard.Add(GetSquadSummary(clubId));

            return dashboard;
        }

        private Group GetSquadSummary(Guid clubId) {
            return new Group { 
                Name = "squads",
                Items = dashboardRepository.GetSquadSummary(clubId)
            };
        }
    }    
}
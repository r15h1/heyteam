using System;
using System.Collections.Generic;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases
{
    public class DashboardUseCase : IUseCase<DashboardRequest, Response<List<Dashboard.Group>>> {
        private readonly IClubRepository clubRepository;
        private readonly AbstractDashboardBuilder dashboardBuilder;
        private readonly IValidator<DashboardRequest> validator;

        public DashboardUseCase(IClubRepository clubRepository, AbstractDashboardBuilder dashboardBuilder, IValidator<DashboardRequest> validator) {
            this.clubRepository = clubRepository;
            this.dashboardBuilder = dashboardBuilder;
            this.validator = validator;
        }

        public Response<List<Dashboard.Group>> Execute(DashboardRequest request) {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<List<Dashboard.Group>>.CreateResponse(validationResult.Messages);
            
            var club = clubRepository.Get(request.ClubId);
            if (club == null)
                return Response<List<Dashboard.Group>>.CreateResponse(new ClubNotFoundException());

            dashboardBuilder.UserEmail = request.UserEmail;
            dashboardBuilder.ClubId = request.ClubId;
            var dashboard = dashboardBuilder.Build();
            return new Response<List<Dashboard.Group>>(dashboard);
        }
    }    
}
using System;
using System.Collections.Generic;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases {
    public class DashboardUseCase : IUseCase<DashboardRequest, Response<List<Dashboard.Group>>> {
        private readonly IUserRepository userRepository;
        private readonly IDashboardRepository dashboardRepository;
        private readonly IValidator<DashboardRequest> validator;

        public DashboardUseCase(IDashboardRepository dashboardRepository, IUserRepository userRepository, IValidator<DashboardRequest> validator) {
            this.userRepository = userRepository;
            this.dashboardRepository = dashboardRepository;
            this.validator = validator;
        }

        public Response<List<Dashboard.Group>> Execute(DashboardRequest request) {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<List<Dashboard.Group>>.CreateResponse(validationResult.Messages);

            //check user's roles
            throw new NotImplementedException();
        }   
    }    
}
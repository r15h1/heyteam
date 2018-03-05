using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeyTeam.Lib.Services
{
    public class ReportDesigner : IReportDesigner
    {
        private readonly IValidator<NewReportDesignRequest> newReportCardDesignValidator;
        private readonly IClubQuery clubQuery;
        private readonly IEvaluationQuery evaluationQuery;
        private readonly IReportDesignerQuery reportDesignerQuery;
        private readonly IReportDesignerRepository reportDesignerRepository;

        public ReportDesigner(
            IValidator<NewReportDesignRequest> newReportCardDesignValidator,
            IClubQuery clubQuery,
            IEvaluationQuery evaluationQuery,
            IReportDesignerQuery reportDesignerQuery,
            IReportDesignerRepository reportDesignerRepository
        )
        {
            this.newReportCardDesignValidator = newReportCardDesignValidator;
            this.clubQuery = clubQuery;
            this.evaluationQuery = evaluationQuery;
            this.reportDesignerQuery = reportDesignerQuery;
            this.reportDesignerRepository = reportDesignerRepository;
        }


        public (Guid Guid, Response Response) CreateReportCardDesign(NewReportDesignRequest request)
        {            
            var validationResult = newReportCardDesignValidator.Validate(request);
            if (!validationResult.IsValid)
                return (Guid.Empty, Response.CreateResponse(validationResult.Messages));

            var club = clubQuery.GetClub(request.ClubId);
            if(club == null)
                return (Guid.Empty, Response.CreateResponse(new EntityNotFoundException("The specified club does not exist")));
                        
            var designs = reportDesignerQuery.GetReportCardDesigns(request.ClubId, request.Name);
            if(designs.Count() > 0)
                return (Guid.Empty, Response.CreateResponse(new IllegalOperationException("There is already a report design with this name")));

            try {
                var design = new ReportCardDesign(request.ClubId) {
                    Name = request.Name
                };
                reportDesignerRepository.AddReportDesign(design);
                return (design.Guid.Value, Response.CreateSuccessResponse());
            } catch(Exception ex) {
                return (Guid.Empty, Response.CreateResponse(ex));
            }           
        }
    }
}
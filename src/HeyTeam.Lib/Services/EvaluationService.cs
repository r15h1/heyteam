using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Lib.Services {
	public class EvaluationService : IEvaluationService {
		private readonly IValidator<TermSetupRequest> setupRequestValidator;
		private readonly IEvaluationQuery evaluationQuery;
		private readonly IEvaluationRepository termRepository;

		public EvaluationService(IValidator<TermSetupRequest> setupRequestValidator, IEvaluationQuery evaluationQuery,
				IEvaluationRepository termRepository) {
			this.setupRequestValidator = setupRequestValidator;
			this.evaluationQuery = evaluationQuery;
			this.termRepository = termRepository;
		}	

		public Response CreateTerm(TermSetupRequest request) {
			var evaluationResult = setupRequestValidator.Validate(request);
			if (!evaluationResult.IsValid)
				return Response.CreateResponse(evaluationResult.Messages);

			if(!request.TermId.IsEmpty())
				return Response.CreateResponse(new IllegalOperationException("No term id must be specified when creating new terms"));

			if(HasOverlappingPeriods(request.ClubId, request.StartDate, request.EndDate))
				return Response.CreateResponse(new IllegalOperationException("This term overlaps with another one"));
			
			if (HasExistingOpenTerms(request.ClubId))
				return Response.CreateResponse(new IllegalOperationException("There can only be one open term at any given time. Please close any existing open term."));
			
			try{
				termRepository.AddTerm(request);
				return Response.CreateSuccessResponse();
			} catch(Exception ex) {
				return Response.CreateResponse(ex);
			}		
		}

		private bool HasExistingOpenTerms(Guid clubId, Guid? ignoredTermId = null) {
			var openTerms = evaluationQuery.GetTerms(clubId, status:TermStatus.Open);
			return ignoredTermId.HasValue ? openTerms.Any(t => t.Guid != ignoredTermId.Value) : openTerms.Any();
		}

		private bool HasOverlappingPeriods(Guid clubId, DateTime startDate, DateTime endDate, Guid? ignoredTermId = null) {
			var overlappingTerms = evaluationQuery.GetTerms(clubId, startDate, endDate);
			return overlappingTerms.Count() > 0;			
		}

		public Response DeleteTerm(TermDeleteRequest request) {
			throw new NotImplementedException();
		}

		public Response UpdateStatus(TermStatusUpdateRequest request) {
			throw new NotImplementedException();
		}

		public Response UpdateTerm(TermSetupRequest request) {
			var evaluationResult = setupRequestValidator.Validate(request);
			if (!evaluationResult.IsValid)
				return Response.CreateResponse(evaluationResult.Messages);

			if (request.TermId.IsEmpty())
				return Response.CreateResponse(new IllegalOperationException("TermId must be specified"));

			var term = evaluationQuery.GetTerm(request.TermId.Value);
			if(term == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified term does not exist"));
			else if (term.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified term does not belong to this club"));

			if (IsTermClosed(term)) 
				return Response.CreateResponse(new IllegalOperationException("A closed term cannot be updated"));
			
			if (HasOverlappingPeriods(request.ClubId, request.StartDate, request.EndDate, request.TermId))
				return Response.CreateResponse(new IllegalOperationException("This term overlaps with another one"));			

			throw new NotImplementedException();
		}

		private bool IsTermClosed(Term term) => term.TermStatus == TermStatus.Closed;

        public (Guid Guid, Response Response) CreateReportCardDesign(NewReportCardDesignRequest request)
        {
            return (Guid.Empty, Response.CreateResponse(new NotImplementedException()));
        }
    }
}

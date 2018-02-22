using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Lib.Queries;
using HeyTeam.Util;
using System;

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

			if(HasOverlappingPeriods(request.PeriodStart, request.PeriodEnd)){
				return Response.CreateResponse(new IllegalOperationException("This term overlaps with another one"));
			}

			if (HasExistingOpenTerms()) {
				return Response.CreateResponse(new IllegalOperationException("There can only be one open term at any given time. Please close any existing open term."));
			}

			throw new NotImplementedException();
		}

		private bool HasExistingOpenTerms(Guid? ignoredTermId = null) {
			throw new NotImplementedException();
		}

		private bool HasOverlappingPeriods(TermPeriod periodStart, TermPeriod periodEnd, Guid? ignoredTermId = null) {
			throw new NotImplementedException();
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
			
			if (HasOverlappingPeriods(request.PeriodStart, request.PeriodEnd, request.TermId))
				return Response.CreateResponse(new IllegalOperationException("This term overlaps with another one"));			

			throw new NotImplementedException();
		}

		private bool IsTermClosed(Term term) => term.TermStatus == TermStatus.CLOSED;
	}
}

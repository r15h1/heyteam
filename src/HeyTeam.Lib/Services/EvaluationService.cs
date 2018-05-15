using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Search;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Lib.Services {
	public class EvaluationService : IEvaluationService {
		private readonly IValidator<TermSetupRequest> setupRequestValidator;
		private readonly IEvaluationQuery evaluationQuery;
		private readonly IEvaluationRepository evaluationRepository;
		private readonly ITermSearchEngine termSearchEngine;
		private readonly IValidator<GenerateReportCardRequest> generateRequestValidator;
		private readonly IClubQuery clubQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;
		private readonly IReportDesignerQuery reportDesignerQuery;
        private readonly IValidator<UpdateReportCardRequest> updateReportCardRequestValidator;

        public EvaluationService(IValidator<TermSetupRequest> setupRequestValidator, IEvaluationQuery evaluationQuery,
				IEvaluationRepository termRepository, ITermSearchEngine termSearchEngine, 
				IValidator<GenerateReportCardRequest> generateRequestValidator, IClubQuery clubQuery,
				ISquadQuery squadQuery, IMemberQuery memberQuery, IReportDesignerQuery reportDesignerQuery,
                IValidator<UpdateReportCardRequest> updateReportCardRequestValidator
        ) {
			this.setupRequestValidator = setupRequestValidator;
			this.evaluationQuery = evaluationQuery;
			this.evaluationRepository = termRepository;
			this.termSearchEngine = termSearchEngine;
			this.generateRequestValidator = generateRequestValidator;
			this.clubQuery = clubQuery;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
			this.reportDesignerQuery = reportDesignerQuery;
            this.updateReportCardRequestValidator = updateReportCardRequestValidator;
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
				evaluationRepository.AddTerm(request);
				termSearchEngine.UpdateCache();
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

		public (Guid? Guid, Response Response) GeneratePlayerReportCard(GenerateReportCardRequest request) {
			var evaluationResult = generateRequestValidator.Validate(request);
			if (!evaluationResult.IsValid)
				return (null, Response.CreateResponse(evaluationResult.Messages));

            var response = CheckReportCardRequestIntegrity(request.ClubId, request.SquadId, request.TermId, request.PlayerId);
            if (!response.RequestIsFulfilled)
                return (null, response);

            var reportDesign = reportDesignerQuery.GetReportCardDesign(request.ClubId, request.ReportDesignId);
			if (reportDesign == null)
				return (null, Response.CreateResponse(new EntityNotFoundException("The specified report design does not exist")));
			else if(reportDesign.ClubId != request.ClubId)
				return (null, Response.CreateResponse(new IllegalOperationException("The specified report design does not belong to this club")));

			var reportCard = evaluationQuery.GetPlayerReportCard(request.ClubId, request.TermId, request.SquadId, request.PlayerId);
			if(reportCard !=null && reportCard.ReportCardExists)
				return (null, Response.CreateResponse(new IllegalOperationException("There is already a report card for this player for the specified term")));

			try {
				Guid reportGuid = evaluationRepository.GeneratePlayerReportCard(request);
				return (reportGuid, Response.CreateSuccessResponse());
			}catch(Exception ex){
				return (null, Response.CreateResponse(ex));
			}
		}

        public Response UpdatePlayerReportCard(UpdateReportCardRequest request)
        {
            var evaluationResult = updateReportCardRequestValidator.Validate(request);
            if (!evaluationResult.IsValid)
                return Response.CreateResponse(evaluationResult.Messages);

            var response = CheckReportCardRequestIntegrity(request.ClubId, request.SquadId, request.TermId, request.PlayerId);
            if (response.Errors.Any())
                return response;

            var reportCard = evaluationQuery.GetPlayerReportCard(request.ClubId, request.ReportCardId);
            if (reportCard == null || !reportCard.ReportCardExists)
                return Response.CreateResponse(new EntityNotFoundException("The specified report card does not exist"));
            else if(reportCard.PlayerId != request.PlayerId)
                return Response.CreateResponse(new IllegalOperationException("The specified report card does not belong to this player"));

            try
            {
                evaluationRepository.UpdatePlayerReportCard(request);
                return Response.CreateSuccessResponse();
            }catch(Exception ex)
            {
                return Response.CreateResponse(ex);
            }
        }

        private Response CheckReportCardRequestIntegrity(Guid clubId, Guid squadId, Guid termId, Guid playerId)
        {
            var club = clubQuery.GetClub(clubId);
            if (club == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

            var squad = squadQuery.GetSquad(squadId);
            if (squad == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified squad does not exist"));
            else if (club.Guid != squad.ClubId)
                return Response.CreateResponse(new IllegalOperationException("The specified squad does not belong to this club"));

            var term = evaluationQuery.GetTerm(termId);
            if (term == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified term does not exist"));
            else if (term.ClubId != club.Guid)
                return Response.CreateResponse(new IllegalOperationException("The specified term does not belong to this club"));
            else if (term.TermStatus == TermStatus.Closed)
                return Response.CreateResponse(new IllegalOperationException("Report cards cannot be generated for closed terms"));

            var player = memberQuery.GetPlayer(playerId);
            if (player == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified player does not exist"));
            else if (player.SquadId != squad.Guid)
                return Response.CreateResponse(new IllegalOperationException("The specified player does not belong to this squad"));

            return Response.CreateSuccessResponse();
        }

        public Response DeleteReportCard(DeleteReportCardRequest request)
        {
            var response = CheckReportCardRequestIntegrity(request.ClubId, request.SquadId, request.TermId, request.PlayerId);
            if (response.Errors.Any())
                return response;

            var reportCard = evaluationQuery.GetPlayerReportCard(request.ClubId, request.ReportCardId);
            if (reportCard == null || !reportCard.ReportCardExists)
                return Response.CreateResponse(new EntityNotFoundException("The specified report card does not exist"));
            else if (reportCard.PlayerId != request.PlayerId)
                return Response.CreateResponse(new IllegalOperationException("The specified report card does not belong to this player"));

            try
            {
                evaluationRepository.DeletePlayerReportCard(request);
                return Response.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                return Response.CreateResponse(ex);
            }
        }
    }
}
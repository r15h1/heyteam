using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Services {
	public class SquadService : ISquadService {
		private readonly ISquadRepository squadRepository;
		private readonly ISquadQuery squadQuery;
		private readonly IClubQuery clubQuery;
		private readonly ICoachQuery coachQuery;

		public SquadService(ISquadRepository squadRepository, ISquadQuery squadQuery, IClubQuery clubQuery, ICoachQuery coachQuery)
		{
			this.squadRepository = squadRepository;
			this.squadQuery = squadQuery;
			this.clubQuery = clubQuery;
			this.coachQuery = coachQuery;
		}

		public Response AssignCoach(Guid squadId, Guid coachId) {
			var squad = squadQuery.GetSquad(squadId);
			if (squad == null)
				return Response.CreateResponse(new List<string> { "The specified squad does not exist" });

			var coach = coachQuery.GetCoach(coachId);
			if (coach == null)
				return Response.CreateResponse(new List<string> { "The specified coach does not exist" });
			else if(squad.ClubId != coach.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The squad and coach belong to different clubs"));

			squadRepository.AssignCoach(squadId, coachId);
			return Response.CreateResponse();
		}

		public Response UnAssignCoach(Guid squadId, Guid coachId) {
			var fullDetails = squadQuery.GetFullSquadDetails(squadId);
			if (fullDetails.Squad == null || fullDetails.Coach == null)
				return Response.CreateResponse(new List<string> { "The specified squad or coach does not exist" });			
			else if(fullDetails.Coach.Guid != coachId)
				return Response.CreateResponse(new IllegalOperationException("The squad's coach is not the one specified"));

			squadRepository.UnAssignCoach(squadId, coachId);
			return Response.CreateResponse();
		}

		public Response RegisterSquad(SquadRequest request) {
			var errors = Validate(request);
			if(errors.Count() > 0)
				return Response.CreateResponse(errors);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			try {				
				squadRepository.AddSquad(new Squad(club.Guid) { Name = request.SquadName });				
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}

			return Response.CreateResponse();
		}

		private IEnumerable<string> Validate(SquadRequest request, bool mustHaveSquadId = false) {
			List<string> errors = new List<string>();
			if (request.ClubId.IsEmpty())
				errors.Add("ClubId must be provided");

			if (request.SquadName.IsEmpty())
				errors.Add("Name must be provided");

			if(mustHaveSquadId && request.SquadId.IsEmpty())
				errors.Add("SquadId must be provided");

			return errors;				
		}

		public Response UpdateSquadProfile(SquadRequest request) {
			var errors = Validate(request, true);
			if (errors.Count() > 0)
				return Response.CreateResponse(errors);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			var squad = squadQuery.GetSquad(request.SquadId.Value);
			if(squad == null || squad.ClubId != request.ClubId)
				return Response.CreateResponse(new EntityNotFoundException("The specified squad does not exist"));

			var squads = squadQuery.GetSquads(request.ClubId);

			try {
				squadRepository.UpdateSquad(new Squad(club.Guid, request.SquadId) { Name = request.SquadName });
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}

			return Response.CreateResponse();
		}
	}
}

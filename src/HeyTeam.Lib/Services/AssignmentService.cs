using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using System.Linq;
using HeyTeam.Util;
using System;
using HeyTeam.Core.Repositories;

namespace HeyTeam.Lib.Services {
	public class AssignmentService : IAssignmentService {
		private readonly IValidator<AssignmentRequest> assignementRequestValidator;
		private readonly IClubQuery clubQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;
		private readonly ILibraryQuery libraryQuery;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly IAssignmentQuery assignmentQuery;

        public AssignmentService(IValidator<AssignmentRequest> assignementRequestValidator, 
			IClubQuery clubQuery,
			ISquadQuery squadQuery,
			IMemberQuery memberQuery,
			ILibraryQuery libraryQuery,
            IAssignmentRepository assignmentRepository,
            IAssignmentQuery assignmentQuery
		){
			this.assignementRequestValidator = assignementRequestValidator;
			this.clubQuery = clubQuery;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
			this.libraryQuery = libraryQuery;
            this.assignmentRepository = assignmentRepository;
            this.assignmentQuery = assignmentQuery;
        }

		public Response CreateAssignment(AssignmentRequest request) {
			var result = assignementRequestValidator.Validate(request);
			if (!result.IsValid)
				return Response.CreateResponse(result.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

            var coach = memberQuery.GetCoach(request.CoachId);
            if(coach == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified coach does not exist"));
            else if(coach.ClubId != club.Guid)
                return Response.CreateResponse(new IllegalOperationException("The specified coach does not belong to this club"));

            var clubSquads = squadQuery.GetSquads(club.Guid);
            if (request.Squads?.Count() > 0)
            {
                var allOfRequestedSquadsBelongToClub = !request.Squads.Except(clubSquads.Select(s => s.Guid)).Any();
                if (!allOfRequestedSquadsBelongToClub)
                    return Response.CreateResponse(new IllegalOperationException("Not all of specified squads belong to this club"));
            }

            if (request.Players?.Count() > 0)
            {
                var members = memberQuery.GetMembers(clubSquads.Select(s => s.Guid));
                var players = members.SelectMany(s => s.Members)?.Where(m => m.Membership.Equals("Player"));

                var allOfRequestedPlayersBelongToClub = !request.Players.Except(players.Select(p => p.Guid)).Any();
                if (!allOfRequestedPlayersBelongToClub)
                    return Response.CreateResponse(new IllegalOperationException("Not all of specified players belong to this club"));
            }

            if (request.TrainingMaterials?.Count() > 0){
				var clubLibrary = libraryQuery.GetTrainingMaterials(request.ClubId);
				var allOfRequestedMaterialsBelongToClub = !request.TrainingMaterials.Except(request.TrainingMaterials).Any();
				if (!allOfRequestedMaterialsBelongToClub)
					return Response.CreateResponse(new IllegalOperationException("Not all of specified training materials belong to this club"));
			}

            try
            {
                assignmentRepository.CreateAssignment(request);
                return Response.CreateSuccessResponse();
            }catch(Exception ex)
            {
                return Response.CreateResponse(ex);
            }
		}

        public Response RemovePlayerFromAssignment(UnAssignPlayerRequest request)
        {
            var club = clubQuery.GetClub(request.ClubId);
            if (club == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

            var assignment = assignmentQuery.GetPlayerAssignment(new PlayerAssignmentRequest { ClubId = request.ClubId, AssignmentId = request.AssignmentId, PlayerId = request.PlayerId });
            if(assignment == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified assignment does not exist"));
            else if (assignment.ClubId != request.ClubId)
                return Response.CreateResponse(new IllegalOperationException("The specified assignment does not belong to this club"));

            try
            {
                assignmentRepository.DeletePlayerAssignment(request);
                return Response.CreateSuccessResponse();
            }catch(Exception ex)
            {
                return Response.CreateResponse(ex);
            }
        }

		public Response UpdateDueDate(AssignmentUpdateRequest request) {
			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			var assignment = assignmentQuery.GetAssignment(request.ClubId, request.AssignmentId);
			if (assignment == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified assignment does not exist"));
			else if (assignment.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified assignment does not belong to this club"));

			if(request.DueDate < DateTime.Today)
				return Response.CreateResponse(new IllegalOperationException("Due Date must be greater than today's date"));

			try {
				assignmentRepository.UpdateDueDate(request);
				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}
	}
}

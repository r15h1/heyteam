using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using System.Linq;
using HeyTeam.Util;

namespace HeyTeam.Lib.Services {
	public class AssignmentService : IAssignmentService {
		private readonly IValidator<AssignmentRequest> assignementRequestValidator;
		private readonly IClubQuery clubQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;
		private readonly ILibraryQuery libraryQuery;

		public AssignmentService(IValidator<AssignmentRequest> assignementRequestValidator, 
			IClubQuery clubQuery,
			ISquadQuery squadQuery,
			IMemberQuery memberQuery,
			ILibraryQuery libraryQuery
		){
			this.assignementRequestValidator = assignementRequestValidator;
			this.clubQuery = clubQuery;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
			this.libraryQuery = libraryQuery;
		}

		public Response CreateAssignment(AssignmentRequest request) {
			var result = assignementRequestValidator.Validate(request);
			if (!result.IsValid)
				return Response.CreateResponse(result.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club doesn not exist"));
				
			var clubSquads = squadQuery.GetSquads(club.Guid);
			if (request.Squads?.Count() > 0) {				
				var allOfRequestedSquadsBelongToClub = !request.Squads.Except(clubSquads.Select(s => s.Guid)).Any();
				if (!allOfRequestedSquadsBelongToClub)
					return Response.CreateResponse(new IllegalOperationException("Not all of specified squads belong to this club"));
			}

			if (request.Players?.Count() > 0) {
				var members = memberQuery.GetMembers(clubSquads.Select(s => s.Guid));
				var players = members.SelectMany(s => s.Members)?.Where(m => m.Membership.Equals("Player"));

				var allOfRequestedPlayersBelongToClub = !request.Players.Except(players.Select(p => p.Guid)).Any();
				if (!allOfRequestedPlayersBelongToClub)
					return Response.CreateResponse(new IllegalOperationException("Not all of specified players belong to this club"));
			}

			if(request.TrainingMaterials?.Count() > 0){
				var clubLibrary = libraryQuery.GetTrainingMaterials(request.ClubId);
				var allOfRequestedMaterialsBelongToClub = !request.TrainingMaterials.Except(request.TrainingMaterials).Any();
				if (!allOfRequestedMaterialsBelongToClub)
					return Response.CreateResponse(new IllegalOperationException("Not all of specified training materials belong to this club"));
			}

			return Response.CreateResponse(new System.NotImplementedException());
		}
	}
}

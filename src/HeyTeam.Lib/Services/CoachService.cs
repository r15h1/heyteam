using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Lib.Services {
	public class CoachService : ICoachService {
		private readonly IClubQuery clubQuery;
		private readonly IMemberQuery memberQuery;
		private readonly ICoachRepository coachRepository;
		private readonly IValidator<CoachRequest> validator;
		private readonly IIdentityManager identityManager;

		public CoachService(IClubQuery clubQuery,  IMemberQuery memberQuery, ICoachRepository coachRepository, IValidator<CoachRequest> validator, IIdentityManager identityManager) {
			ThrowIf.ArgumentIsNull(clubQuery);
			ThrowIf.ArgumentIsNull(coachRepository);
			ThrowIf.ArgumentIsNull(validator);

			this.clubQuery = clubQuery;
			this.memberQuery = memberQuery;
			this.coachRepository = coachRepository;
			this.validator = validator;
			this.identityManager = identityManager;
		}

		public Response DeleteCoach(DeleteCoachRequest request) {
			var coach = memberQuery.GetCoach(request.CoachId);

			if (coach == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified coach does not exist"));

			if (coach.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified coach does not belong to this club"));

			try {
				coachRepository.DeleteCoach(coach);
				var members = memberQuery.GetMembersByEmail(request.ClubId, coach.Email);
				if (members.Count() == 0) {
					var result = identityManager.RemoveUser(coach.Email).Result;
				} else if (!members.Any(m => m.Membership == Membership.Coach)) {
					var result = identityManager.RemoveUserRole(coach.Email, Membership.Coach).Result;
				}

				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		public Response RegisterCoach(CoachRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if(club ==null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			try {
				coachRepository.AddCoach(MapCoach(request, null));
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateSuccessResponse();
		}

		public Response UpdateCoachProfile(CoachRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			var coach = memberQuery.GetCoach(request.CoachId.Value);
			if(coach == null || coach.ClubId != request.ClubId)
				return Response.CreateResponse(new EntityNotFoundException("The specified coach does not exist"));

			try {
				coachRepository.UpdateCoach(MapCoach(request, request.CoachId));
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateSuccessResponse();
		}		

		private Coach MapCoach(CoachRequest request, Guid? coachId) =>
			new Coach(request.ClubId, coachId) {
				DateOfBirth = request.DateOfBirth,
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Phone = request.Phone,
				Qualifications = request.Qualifications
			};
	}
}

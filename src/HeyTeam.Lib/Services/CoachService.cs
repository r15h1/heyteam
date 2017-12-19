using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Services {
	public class CoachService : ICoachService {
		private readonly IClubQuery clubQuery;
		private readonly IMemberQuery memberQuery;
		private readonly ICoachRepository coachRepository;
		private readonly IValidator<CoachRequest> validator;

		public CoachService(IClubQuery clubQuery,  IMemberQuery memberQuery, ICoachRepository coachRepository, IValidator<CoachRequest> validator) {
			ThrowIf.ArgumentIsNull(clubQuery);
			ThrowIf.ArgumentIsNull(coachRepository);
			ThrowIf.ArgumentIsNull(validator);

			this.clubQuery = clubQuery;
			this.memberQuery = memberQuery;
			this.coachRepository = coachRepository;
			this.validator = validator;
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

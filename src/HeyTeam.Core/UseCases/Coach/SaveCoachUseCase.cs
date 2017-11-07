using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.UseCases.Coach {
    public class SaveCoachUseCase : IUseCase<SaveCoachRequest, Response<Guid?>> {
		private readonly IClubRepository clubRepository;
		private readonly ICoachRepository coachRepository;
		private readonly IValidator<SaveCoachRequest> validator;

		public SaveCoachUseCase(IClubRepository clubRepository, ICoachRepository coachRepository, IValidator<SaveCoachRequest> validator) {
			Ensure.ArgumentNotNull(clubRepository);
			Ensure.ArgumentNotNull(coachRepository);
			Ensure.ArgumentNotNull(validator);

			this.clubRepository = clubRepository;
			this.coachRepository = coachRepository;
			this.validator = validator;
		}

		public Response<Guid?> Execute(SaveCoachRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response<Guid?>.CreateResponse(validationResult.Messages);

			var club = clubRepository.GetClub(request.ClubId);
			if(club ==null)
				return Response<Guid?>.CreateResponse(new EntityNotFoundException());

			var coach = MapCoach(request);

			if (request.Command == SaveCoachRequest.Action.ADD) {
				var coachWithSameId = coachRepository.GetCoach(coach.Guid);
				if (coachWithSameId != null)
					return Response<Guid?>.CreateResponse(new DuplicateEntryException("A coach with this id exists already"));
			}

			try {
				if (request.Command == SaveCoachRequest.Action.UPDATE) coachRepository.UpdateCoach(coach);
				else coachRepository.AddCoach(coach);
			} catch (Exception ex) {
				return Response<Guid?>.CreateResponse(ex);
			}
			return new Response<Guid?>(coach.Guid);
		}

		private Entities.Coach MapCoach(SaveCoachRequest request) =>
			new Entities.Coach(request.ClubId, request.CoachId) {
				DateOfBirth = request.DateOfBirth,
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Phone = request.Phone,
				Qualifications = request.Qualifications
			};
	}
}

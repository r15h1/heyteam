using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.UseCases.Coach {
    public class AddCoachUseCase : IUseCase<AddCoachRequest, Response<Guid?>> {
		private readonly IClubRepository clubRepository;
		private readonly ICoachRepository coachRepository;
		private readonly IValidator<AddCoachRequest> validator;

		public AddCoachUseCase(IClubRepository clubRepository, ICoachRepository coachRepository, IValidator<AddCoachRequest> validator) {
			Ensure.ArgumentNotNull(clubRepository);
			Ensure.ArgumentNotNull(coachRepository);
			Ensure.ArgumentNotNull(validator);

			this.clubRepository = clubRepository;
			this.coachRepository = coachRepository;
			this.validator = validator;
		}

		public Response<Guid?> Execute(AddCoachRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response<Guid?>.CreateResponse(validationResult.Messages);

			var club = clubRepository.GetClub(request.ClubId);
			if(club ==null)
				return Response<Guid?>.CreateResponse(new EntityNotFoundException());

			var coach = MapCoach(request);
			var coachWithSameId = coachRepository.GetCoach(coach.Guid);
			if (coachWithSameId != null)
				return Response<Guid?>.CreateResponse(new DuplicateEntryException("A coach with this id exists already"));

			try {
				coachRepository.AddCoach(coach);
			} catch (Exception ex) {
				return Response<Guid?>.CreateResponse(ex);
			}
			return new Response<Guid?>(coach.Guid);
		}

		private Entities.Coach MapCoach(AddCoachRequest request) =>
			new Entities.Coach(request.ClubId) {
				DateOfBirth = request.DateOfBirth,
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Phone = request.Phone,
				Qualifications = request.Qualifications
			};
	}
}

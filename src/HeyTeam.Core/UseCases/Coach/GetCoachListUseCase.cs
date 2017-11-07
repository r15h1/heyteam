using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using System.Collections.Generic;

namespace HeyTeam.Core.UseCases.Coach {
	public class GetCoachListUseCase : IUseCase<GetCoachListRequest, Response<IEnumerable<Entities.Coach>>> {
		private readonly ICoachRepository coachRepository;
		private readonly IValidator<GetCoachListRequest> validator;

		public GetCoachListUseCase(ICoachRepository coachRepository, IValidator<GetCoachListRequest> validator) {
			this.coachRepository = coachRepository;
			this.validator = validator;
		}

		public Response<IEnumerable<Entities.Coach>> Execute(GetCoachListRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response<IEnumerable<Entities.Coach>>.CreateResponse(validationResult.Messages);

			return new Response<IEnumerable<Entities.Coach>>(coachRepository.GetCoaches(request.ClubId));
		}
	}
}

using HeyTeam.Core.Entities;
using HeyTeam.Core.Events;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using System;
using System.Linq;

namespace HeyTeam.Core.UseCases.Squad {
	public class SquadCoachAssignmentInteractor : IUseCase<SquadCoachChangeRequest, Response<string>> {
		private readonly ISquadRepository squadRepository;
		private readonly ICoachRepository coachRepository;
		private readonly IValidator<SquadCoachChangeRequest> validator;
		private SquadCoachAssignment squadAssignment;

		public SquadCoachAssignmentInteractor(ISquadRepository squadRepository, ICoachRepository coachRepository, IValidator<SquadCoachChangeRequest> validator) {
			this.squadRepository = squadRepository;
			this.coachRepository = coachRepository;
			this.validator = validator;
		}		

		public Response<string> Execute(SquadCoachChangeRequest request) {
			var response = new Response<string>();
			var validationResult = validator.Validate(request);

			if (!validationResult.IsValid)
				return Response<string>.CreateResponse(validationResult.Messages);
			
			var squad = squadRepository.GetSquad(request.SquadId);
			
			if(squad == null)
				return Response<string>.CreateResponse(new EntityNotFoundException("Squad does not exist"));

			var coaches = coachRepository.GetSquadCoaches(request.SquadId).ToList();
			squadAssignment = new SquadCoachAssignment(squad, coaches);
			
			var coach = coachRepository.GetCoach(request.CoachId);
			if (coach == null)
				return Response<string>.CreateResponse(new EntityNotFoundException("Coach does not exist"));

			squadAssignment.OnCoachAdded += SquadCoachAdded;
			squadAssignment.OnCoachRemoved += SquadCoachRemoved;
			try {
				if (request.Operation == SquadCoachChangeRequest.SquadCoachOperation.ADD)
					squadAssignment.AddCoach(coach);
				else
					squadAssignment.RemoveCoach(coach);
			} catch(Exception ex) {
				return Response<string>.CreateResponse(ex);
			}

			return response;
		}

		public void SquadCoachAdded(SquadCoachChanged sq) {
			squadRepository.AssignCoach(sq.SquadId, sq.CoachId);
		}

		public void SquadCoachRemoved(SquadCoachChanged sq) {
			squadRepository.UnAssignCoach(sq.SquadId, sq.CoachId);
		}
	}
}
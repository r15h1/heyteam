using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Lib.Services {
	public class EventService : IEventService {
		private readonly IEventRepository eventRepository;
		private readonly IValidator<EventSetupRequest> validator;
		private readonly IClubQuery clubQuery;
		private readonly ISquadQuery squadQuery;

		public EventService(IEventRepository eventRepository, IValidator<EventSetupRequest> validator, IClubQuery clubQuery, ISquadQuery squadQuery) {
			ThrowIf.ArgumentIsNull(eventRepository);
			ThrowIf.ArgumentIsNull(clubQuery);
			ThrowIf.ArgumentIsNull(squadQuery);
			ThrowIf.ArgumentIsNull(validator);
			this.eventRepository = eventRepository;
			this.validator = validator;
			this.clubQuery = clubQuery;
			this.squadQuery = squadQuery;
		}

		public Response CreateEvent(EventSetupRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				throw new EntityNotFoundException("The specified club doesn not exist");

			var squads = squadQuery.GetSquads(club.Guid);
			var similar = request.Squads.Intersect(squads.Select(s => s.Guid));
						
			try{
				eventRepository.AddEvent(Map(request));
			} catch(Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateResponse();
		}

		private Event Map(EventSetupRequest request) => new Event(request.ClubId) {
			EndDate = request.EndDate.Value,
			Location = request.Location,
			StartDate = request.StartDate.Value,
			Title = request.Title
		};
	}
}

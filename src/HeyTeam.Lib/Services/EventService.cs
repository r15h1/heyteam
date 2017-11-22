using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Services {
	public class EventService : IEventService {
		private readonly IEventRepository eventRepository;
		private readonly IEventQuery eventQuery;
		private readonly IValidator<EventSetupRequest> setUpRequestValidator;
		private readonly IValidator<EventDeleteRequest> deleteRequestValidator;
		private readonly IClubQuery clubQuery;
		private readonly ISquadQuery squadQuery;

		public EventService(IEventRepository eventRepository, IEventQuery eventQuery, IValidator<EventSetupRequest> setUpRequestValidator, 
								IValidator<EventDeleteRequest> deleteRequestValidator, IClubQuery clubQuery, ISquadQuery squadQuery
		) {
			ThrowIf.ArgumentIsNull(eventRepository);
			ThrowIf.ArgumentIsNull(eventQuery);
			ThrowIf.ArgumentIsNull(clubQuery);
			ThrowIf.ArgumentIsNull(squadQuery);
			ThrowIf.ArgumentIsNull(setUpRequestValidator);
			this.eventRepository = eventRepository;
			this.eventQuery = eventQuery;
			this.setUpRequestValidator = setUpRequestValidator;
			this.deleteRequestValidator = deleteRequestValidator;
			this.clubQuery = clubQuery;
			this.squadQuery = squadQuery;
		}

		public Response CreateEvent(EventSetupRequest request) {
			if(!request.EventId.IsEmpty())
				return Response.CreateResponse(new IllegalOperationException("Event Id must not be specified when creating a new event"));

			var (isValid, squads, response) = Validate(request);
			if (!isValid) return response;

			try {
				eventRepository.AddEvent(Map(request, squads));
			} catch(Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateResponse();
		}

		public Response UpdateEvent(EventSetupRequest request) {
			if (request.EventId.IsEmpty())
				return Response.CreateResponse(new EntityNotFoundException("Event Id must be specified when updating an existing event"));
			else {
				var @event = eventQuery.GetEvent(request.EventId.Value);
				if(@event == null)
					return Response.CreateResponse(new EntityNotFoundException("The specified event does not exist"));
				else if (@event.ClubId != request.ClubId)
					return Response.CreateResponse(new IllegalOperationException("The specified event does not belong to this club"));
			}
			
			var (isValid, squads, response) = Validate(request);
			if (!isValid) return response;

			try {
				eventRepository.UpdateEvent(Map(request, squads));
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateResponse();
		}

		private (bool isValid, IEnumerable<Squad> clubSquads, Response response) Validate(EventSetupRequest request) {
			var validationResult = setUpRequestValidator.Validate(request);
			if (!validationResult.IsValid)
				return (false, null, Response.CreateResponse(validationResult.Messages));

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return (false, null, Response.CreateResponse(new EntityNotFoundException("The specified club doesn not exist")));

			var clubSquads = squadQuery.GetSquads(club.Guid);
			var allOfRequestedSquadsBelongToClub = !request.Squads.Except(clubSquads.Select(s => s.Guid)).Any();
			if (!allOfRequestedSquadsBelongToClub)
				return (false, null, Response.CreateResponse(new IllegalOperationException("Not all of specified squads belong to this club")));

			var squads = request.Squads.Join(clubSquads, s1 => s1, s2 => s2.Guid, (guid, squad) => squad).ToList();
			return (true, squads, Response.CreateResponse());
		}


		private Event Map(EventSetupRequest request, IEnumerable<Squad> squads) => new Event(request.ClubId, request.EventId) {
			EndDate = request.EndDate.Value,
			Location = request.Location,
			StartDate = request.StartDate.Value,
			Title = request.Title,
			Squads = squads
		};

		public Response DeleteEvent(EventDeleteRequest request) {
			var validationResult = deleteRequestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);
				
			var @event = eventQuery.GetEvent(request.EventId);
			if(@event == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified event was not found"));
			else if (@event.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified event belongs does not belong to this club"));


			try {
				eventRepository.DeleteEvent(request.ClubId, request.EventId);
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateResponse();
		}
	}
}

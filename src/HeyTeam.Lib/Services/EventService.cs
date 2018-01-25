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
		private readonly ILibraryQuery libraryQuery;
		private readonly IMemberQuery memberQuery;
		private readonly IValidator<EventAttendanceRequest> eventAttendanceRequestValidator;

		public EventService(IEventRepository eventRepository, IEventQuery eventQuery, IValidator<EventSetupRequest> setUpRequestValidator, 			
								IValidator<EventDeleteRequest> deleteRequestValidator, IClubQuery clubQuery, ISquadQuery squadQuery, ILibraryQuery libraryQuery, IMemberQuery memberQuery,
								IValidator<EventAttendanceRequest> eventAttendanceRequestValidator
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
			this.libraryQuery = libraryQuery;
			this.memberQuery = memberQuery;
			this.eventAttendanceRequestValidator = eventAttendanceRequestValidator;
		}

		public Response CreateEvent(EventSetupRequest request) {
			if(!request.EventId.IsEmpty())
				return Response.CreateResponse(new IllegalOperationException("Event Id must not be specified when creating a new event"));

			var (isValid, squads, trainingMaterials, response) = Validate(request);
			if (!isValid) return response;

			try {
				eventRepository.AddEvent(Map(request, squads, trainingMaterials));
			} catch(Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateSuccessResponse();
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
			
			var (isValid, squads, trainingMaterials, response) = Validate(request);
			if (!isValid) return response;

			try {
				eventRepository.UpdateEvent(Map(request, squads, trainingMaterials));
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateSuccessResponse();
		}

		private (bool isValid, IEnumerable<Squad> squads, IEnumerable<TrainingMaterial> trainingMaterials, Response response) Validate(EventSetupRequest request) {
			var validationResult = setUpRequestValidator.Validate(request);
			if (!validationResult.IsValid)
				return (false, null, null, Response.CreateResponse(validationResult.Messages));

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return (false, null, null, Response.CreateResponse(new EntityNotFoundException("The specified club doesn not exist")));

			var clubSquads = squadQuery.GetSquads(club.Guid);
			var allOfRequestedSquadsBelongToClub = !request.Squads.Except(clubSquads.Select(s => s.Guid)).Any();
			if (!allOfRequestedSquadsBelongToClub)
				return (false, null, null, Response.CreateResponse(new IllegalOperationException("Not all of specified squads belong to this club")));

			List<TrainingMaterial> trainingMaterials = null;
			if (request.TrainingMaterials != null && request.TrainingMaterials.Any()) {
				var clubTrainingMaterials = libraryQuery.GetTrainingMaterials(club.Guid);
				var allOfRequestedMaterialsBelongToClub = !request.TrainingMaterials.Except(clubTrainingMaterials.Select(t => t.Guid)).Any();
				if (!allOfRequestedMaterialsBelongToClub)
					return (false, null, null, Response.CreateResponse(new IllegalOperationException("Not all of specified materials belong to this club")));

				trainingMaterials = request.TrainingMaterials.Join(clubTrainingMaterials, t1 => t1, t2 => t2.Guid, (guid, trainignMaterial) => trainignMaterial).ToList();
			}

			var squads = request.Squads.Join(clubSquads, s1 => s1, s2 => s2.Guid, (guid, squad) => squad).ToList();			
			return (true, squads, trainingMaterials, Response.CreateSuccessResponse());
		}


		private Event Map(EventSetupRequest request, IEnumerable<Squad> squads, IEnumerable<TrainingMaterial> trainingMaterials) => new Event(request.ClubId, request.EventId) {
			EndDate = request.EndDate.Value,
			Location = request.Location,
			StartDate = request.StartDate.Value,
			Title = request.Title,
			Squads = squads,
			TrainingMaterials = trainingMaterials
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
			return Response.CreateSuccessResponse();
		}

		public Response UpdateAttendance(EventAttendanceRequest request) {
			var validationResult = eventAttendanceRequestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			var squad = squadQuery.GetSquad(request.SquadId);			
			if (squad == null || squad.ClubId != club.Guid)
				return Response.CreateResponse(new IllegalOperationException("The specified squad does not belong to this club"));

			var @event = eventQuery.GetEvent(request.EventId);
			if(@event == null || !@event.Squads.Any(s => s.Guid == squad.Guid))
				return Response.CreateResponse(new IllegalOperationException("The specified squad is not bound to this event"));

			var player = memberQuery.GetPlayer(request.PlayerId);
			if(player == null || player.SquadId != squad.Guid)
				return Response.CreateResponse(new IllegalOperationException("The specified player does not belong this squad"));

			try {
				eventRepository.UpdateAttendance(request.SquadId, request.EventId, request.PlayerId, request.Attendance);
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
			return Response.CreateSuccessResponse();
		}
	}
}

using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeyTeam.Lib.Services
{
    public class Tracker : ITracker
    {
        private readonly IValidator<EventTrainingMaterialViewRequest> eventTrainingMaterialViewValidator;
        private readonly IClubQuery clubQuery;
        private readonly IEventQuery eventQuery;
        private readonly ILibraryQuery libraryQuery;
        private readonly IMemberQuery memberQuery;
        private readonly ISquadQuery squadQuery;
		private readonly IAssignmentQuery assignmentQuery;
		private readonly ITrackerRepository trackerRepository;

        public Tracker(IValidator<EventTrainingMaterialViewRequest> eventTrainingMaterialViewValidator, IClubQuery clubQuery, IEventQuery eventQuery, 
            ILibraryQuery libraryQuery, IMemberQuery memberQuery, ISquadQuery squadQuery, IAssignmentQuery assignmentQuery, ITrackerRepository trackerRepository)
        {
            this.eventTrainingMaterialViewValidator = eventTrainingMaterialViewValidator;
            this.clubQuery = clubQuery;
            this.eventQuery = eventQuery;
            this.libraryQuery = libraryQuery;
            this.memberQuery = memberQuery;
            this.squadQuery = squadQuery;
			this.assignmentQuery = assignmentQuery;
			this.trackerRepository = trackerRepository;
        }

        public Response Track(EventTrainingMaterialViewRequest request)
        {
            var validationResult = eventTrainingMaterialViewValidator.Validate(request);
            if (!validationResult.IsValid)
                return Response.CreateResponse(validationResult.Messages);

            var club = clubQuery.GetClub(request.ClubId);
            if (club == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified club doesn not exist"));

            var @event = eventQuery.GetEvent(request.EventId);
            if (@event == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified event was not found"));
            else if (@event.ClubId != request.ClubId)
                return Response.CreateResponse(new IllegalOperationException("The specified event does not belong to this club"));
            else if (!@event.Squads.Any())
                return Response.CreateResponse(new IllegalOperationException("The specified event is not attributed to any squad"));

            var trainingMaterial = libraryQuery.GetTrainingMaterial(request.TrainingMaterialId);
            if (trainingMaterial == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified training material does not exist"));
            else if (!@event.TrainingMaterials.Any(t => t.Guid == request.TrainingMaterialId))
                return Response.CreateResponse(new IllegalOperationException("The specified training material does not belong to this event"));

            var clubSquads = squadQuery.GetSquads(request.ClubId);
            Member member = request.Membership == Core.Membership.Coach ? memberQuery.GetCoach(request.MemberId) as Member : memberQuery.GetPlayer(request.MemberId) as Member;
            var l1 = member.Squads.Intersect(clubSquads.Select(s => s.Guid)).ToList();
            var l2 = clubSquads.Select(s => s.Guid).Intersect(member.Squads).ToList();

            if (member == null)
                return Response.CreateResponse(new EntityNotFoundException("The specified member does not exist"));
            else if (clubSquads == null || !member.Squads.Intersect(clubSquads.Select(s => s.Guid)).Any())
                return Response.CreateResponse(new IllegalOperationException("The specified member does not belong to any squad"));
            else if (!@event.Squads.Select(s => s.Guid).Intersect(member.Squads).Any())
                return Response.CreateResponse(new IllegalOperationException("This member is not concerned by this event"));

            try
            {
                trackerRepository.Track(request);
                return Response.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                return Response.CreateResponse(ex);
            }
        }

        public Response Track(AssignmentTrainingMaterialViewRequest request)
        {
			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club doesn not exist"));

			var assignment = assignmentQuery.GetAssignment(club.Guid, request.AssignmentId);
			if (assignment == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified assignment was not found"));
			else if (assignment.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified assignment does not belong to this club"));

			var trainingMaterial = libraryQuery.GetTrainingMaterial(request.TrainingMaterialId);
			if (trainingMaterial == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified training material does not exist"));
			else if (!assignment.TrainingMaterials.Any(t => t.Guid == request.TrainingMaterialId))
				return Response.CreateResponse(new IllegalOperationException("The specified training material does not belong to this assignment"));

			var clubSquads = squadQuery.GetSquads(request.ClubId);
			Member member = request.Membership == Core.Membership.Coach ? memberQuery.GetCoach(request.MemberId) as Member : memberQuery.GetPlayer(request.MemberId) as Member;
			var l1 = member.Squads.Intersect(clubSquads.Select(s => s.Guid)).ToList();
			var l2 = clubSquads.Select(s => s.Guid).Intersect(member.Squads).ToList();

			if (member == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified member does not exist"));
			else if (clubSquads == null || !member.Squads.Intersect(clubSquads.Select(s => s.Guid)).Any())
				return Response.CreateResponse(new IllegalOperationException("The specified member does not belong to any squad"));
			

			try {
				trackerRepository.Track(request);
				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}
    }
}

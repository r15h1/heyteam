using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.TrackingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Controllers.Coaches
{
    [Authorize(Policy = "Coach")]
    [Area("Coaches")]
    [Route("[area]/{memberid:guid}/[controller]")]
	public class TrackingController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly Club club;
        private readonly ITracker tracker;
        private readonly IMemberQuery memberQuery;
        private readonly IEventQuery eventQuery;
        private readonly ILibraryQuery libraryQuery;

		public TrackingController(Club club, ITracker tracker, IMemberQuery memberQuery, IEventQuery eventQuery, ILibraryQuery libraryQuery) {
            this.club = club;
            this.tracker = tracker;
            this.memberQuery = memberQuery;
            this.eventQuery = eventQuery;
            this.libraryQuery= libraryQuery;
		}

        [HttpGet("events/{eventId:guid}/library/{trainingMaterialId:guid}")]
		public IActionResult EventTrainingMaterialView(LibraryTrackingViewModel model)
        {
            var response = tracker.Track(new EventTrainingMaterialViewRequest{ClubId = club.Guid, EventId = model.SourceId, MemberId=model.MemberId, TrainingMaterialId= model.TrainingMaterialId, Membership = Core.Membership.Coach });
            if (response.RequestIsFulfilled)
            {
                var trainingMaterial = libraryQuery.GetTrainingMaterial(model.TrainingMaterialId);
                return Redirect(trainingMaterial.Url);
            }
            return View("ResourceNotFound");
        }

        [HttpGet("asssignments/{eventId:guid}/library/{trainingMaterialId:guid}")]
        public IActionResult AssignmentTrainingMaterialView(LibraryTrackingViewModel model)
        {
            var response = tracker.Track(new EventTrainingMaterialViewRequest { ClubId = club.Guid, EventId = model.SourceId, MemberId = model.MemberId, TrainingMaterialId = model.TrainingMaterialId, Membership = Core.Membership.Player });
            if (response.RequestIsFulfilled)
            {
                var trainingMaterial = libraryQuery.GetTrainingMaterial(model.TrainingMaterialId);
                return Redirect(trainingMaterial.Url);
            }
            return View("ResourceNotFound");
        }
    }
}
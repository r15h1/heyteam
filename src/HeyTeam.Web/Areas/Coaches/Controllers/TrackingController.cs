using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.TrackingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HeyTeam.Web.Controllers {
    [Authorize(Policy = "Coach")]
    [Area("Coaches")]
    [Route("[area]/{memberid:guid}/[controller]")]
	public class TrackingController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ITracker tracker;
        private readonly IMemberQuery memberQuery;
        private readonly IEventQuery eventQuery;
        private readonly ILibraryQuery libraryQuery;

		public TrackingController(ITracker tracker, IMemberQuery memberQuery, IEventQuery eventQuery, ILibraryQuery libraryQuery) {
            this.tracker = tracker;
            this.memberQuery = memberQuery;
            this.eventQuery = eventQuery;
            this.libraryQuery= libraryQuery;
		}

        [HttpGet("events/{eventId:guid}/library/{trainingMaterialId:guid}")]
		public IActionResult Library(LibraryTrackingViewModel model)
        {
			var trainingMaterial = libraryQuery.GetTrainingMaterial(model.TrainingMaterialId);
            var user = memberQuery.GetCoach(model.MemberId);
            var @event = eventQuery.GetEvent(model.EventId);

			if(trainingMaterial != null)
            {
                tracker.Track(new TrackLibraryRequest{ EventId = model.EventId, MemberId=model.MemberId, TrainingMaterialId= model.TrainingMaterialId, Membership = Core.Membership.Coach });
                return Redirect(trainingMaterial.Url);
            }
				
			
            return View();
        }
    }
}
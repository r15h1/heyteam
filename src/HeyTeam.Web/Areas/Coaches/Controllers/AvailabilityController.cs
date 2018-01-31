using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.AvailabilityViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace HeyTeam.Web.Areas.Coaches.Controllers {
	[Authorize(Policy = "Coach")]
	[Area("Coaches")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class AvailabilityController : Controller
    {
		private readonly Club club;
		private readonly IAvailabilityQuery availabilityQuery;
		private readonly IAvailabilityService availabilityService;

		public AvailabilityController(Club club, IAvailabilityQuery availabilityQuery, IAvailabilityService availabilityService){
			this.club = club;
			this.availabilityQuery = availabilityQuery;
			this.availabilityService = availabilityService;
		}

		[HttpGet("")]
        public IActionResult Index() {
			var availabilities = availabilityQuery.GetAvailabilities(new GetAvailabilityRequest { ClubId = club.Guid });
            return View(availabilities);
        }

		[HttpGet("new")]
		public IActionResult New() {
			return View();
		}

		[HttpPost("new")]
		public IActionResult New(NewAvailabilityViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var request = new NewAvailabilityRequest { 
				AvailabilityStatus = model.AvailabilityStatus.Value,
				ClubId = club.Guid,
				DateFrom = model.DateFrom.Value,
				DateTo = model.DateTo,
				Notes = model.Notes,
				PlayerId = model.PlayerId
			};

			var response = availabilityService.AddAvailability(request);
			if(!response.RequestIsFulfilled){
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}				

			return RedirectToAction(nameof(Index));
		}

		[HttpGet("{availabilityId:guid}")]
		public IActionResult Edit(Guid availabilityId) {
			var availability = availabilityQuery.GetAvailability(club.Guid, availabilityId);
			var model = new EditAvailabilityViewModel {
				AvailabilityId = availability.AvailabilityId,
				AvailabilityStatus = availability.AvailabilityStatus,
				DateFrom = availability.DateFrom,
				DateTo = availability.DateTo,
				Notes = availability.Notes,
				PlayerId = availability.PlayerId,
				SelectedPlayer = JsonConvert.SerializeObject(
					 new { Id = availability.PlayerId, Text = availability.PlayerName },
					new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
				),
				PlayerName = availability.PlayerName
			};
			return View(model);
		}

		[HttpPost("{availabilityId:guid}")]
		public IActionResult Edit(EditAvailabilityViewModel model) {
			if(!ModelState.IsValid)
				return View(model);

			var request = new UpdateAvailabilityRequest {
				AvailabilityStatus = model.AvailabilityStatus.Value,
				AvailabilityId = model.AvailabilityId,
				ClubId = club.Guid,
				DateFrom = model.DateFrom.Value,
				DateTo = model.DateTo,
				Notes = model.Notes,
				PlayerId = model.PlayerId
			};

			var response = availabilityService.UpdateAvailability(request);
			if (!response.RequestIsFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost("delete")]
        public IActionResult Delete(DeleteAvailabilityViewModel model)
        {
            var response = availabilityService.DeleteAvailability(new DeleteAvailabilityRequest { AvailabilityId=model.AvailabilityId, ClubId = club.Guid, PlayerId = model.PlayerId });
            if (!response.RequestIsFulfilled)
            {
                foreach (var error in response.Errors)
                    ModelState.AddModelError("", error);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

		[HttpGet("history")]
		public IActionResult History() {
			var availabilities = availabilityQuery.GetAvailabilities(new GetAvailabilityRequest { ClubId = club.Guid });
			return View(availabilities);
		}
	}
}
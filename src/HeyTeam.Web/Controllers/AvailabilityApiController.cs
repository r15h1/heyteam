using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.AvailabilityViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HeyTeam.Web.Controllers {
	[Produces("application/json")]
    [Route("api/availabilities")]
    public class AvailabilityApiController : Controller
    {
		private readonly Club club;
		private readonly IAvailabilityQuery availabilityQuery;

		public AvailabilityApiController(Club club, IAvailabilityQuery availabilityQuery) {
			this.club = club;
			this.availabilityQuery = availabilityQuery;
		}

		[HttpGet("")]
		public IActionResult Get(AvailabilityHistoryModel model) {
			var availabilities = availabilityQuery.GetAvailabilities(new GetAvailabilityRequest { ClubId = club.Guid, PlayerId = model.PlayerId, Year = model.Year });
			var response = availabilities.Select(a => 
				new { AvailabilityId = a.AvailabilityId, availabilityStatusId = a.AvailabilityStatus, 
					availabilityStatusDescription = (a.AvailabilityStatus == AvailabilityStatus.Injured ? "Injured" : 
													(a.AvailabilityStatus == AvailabilityStatus.OutOfTown ? "Out of town" : 
													(a.AvailabilityStatus == AvailabilityStatus.Other ? "Other" : "Unknown"))),
					PlayerId = a.PlayerId, PlayerName = a.PlayerName,
					FormattedDateFrom = a.DateFrom.ToString("dd MMM yyyy"), FormattedDateTo = a.DateTo?.ToString("dd MMM yyyy"),
					DateFrom = a.DateFrom.ToString("yyyy-MM-dd"), DateTo = a.DateTo?.ToString("yyyy-MM-dd"),
					Notes = a.Notes,
					squadName = a.SquadName			
			}).ToList();
			return Json(response);
		}

	}
}
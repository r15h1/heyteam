using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HeyTeam.Web.Areas.Coaches.Controllers {
	[Produces("application/json")]    
	[Authorize(Policy = "Coach")]
	[Area("Coaches")]
	[Route("api/[area]/{memberid:guid}")]
	public class DashboardApiController : Controller
    {
		private readonly Club club;
		private readonly IEventQuery eventQuery;

		public DashboardApiController(Club club, IEventQuery eventQuery){
			this.club = club;
			this.eventQuery = eventQuery;
		}
		
		[HttpGet("upcoming-events")]
		public IActionResult GetUpcomingEvents(DashboardModel model)
		{
			var events = eventQuery.GetUpcomingEvents(new UpcomingEventsRequest { Membership = Membership.Coach, MemberId = model.MemberId, ClubId = club.Guid });
			var response = events.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate).Select(e => new {
				EventId = e.Guid, Title = e.Title, TrainingMaterialsCount = e.TrainingMaterialsCount,
				Squads = e.Squads, Location = e.Location, FormattedStartDate = $"{e.StartDate.ToString("dd MMM yyyy h:mm tt")}",
				FormattedEndDate = $"{e.EndDate.ToString("dd MMM yyyy h:mm tt")}",
				StartDate = e.StartDate, EndDate = e.EndDate,
				Attendance = e.Attendance,
				EventType = (byte)e.EventType,
				EventTypeDescription = e.EventType.GetDescription()
			}).ToList();
			return new JsonResult(response);
		}
    }
}
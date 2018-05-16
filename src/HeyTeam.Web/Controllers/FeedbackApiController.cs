using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HeyTeam.Util;

namespace HeyTeam.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/feedback")]
    public class FeedbackApiController : Controller
    {
        private Club club;
        private readonly IFeedbackQuery feedbackQuery;
        private readonly IFeedbackRepository feedbackRepository;
        private readonly IMemberQuery memberQuery;

        public FeedbackApiController(Club club, IFeedbackQuery feedbackQuery, IFeedbackRepository feedbackRepository, IMemberQuery memberQuery)
        {
            this.club = club;
            this.feedbackQuery = feedbackQuery;
            this.feedbackRepository = feedbackRepository;
            this.memberQuery = memberQuery;
        }

        [HttpGet("")]
        public IActionResult GetFeedbacks(FeedbackSearchModel model)
        {
            var feeback = feedbackQuery.GetFeedbackList(
                new FeedbackListRequest {
                    ClubId = club.Guid,
                    SquadId = model.SquadId,
                    Week = model.Week, 
                    Year = model.Year
                }
            );
            return new JsonResult(feeback);
        }

        [HttpPut("publish")]
        public IActionResult PublishFeedback(FeedbackPublishModel model)
        {
            var email = User.Identity.Name;
            var members = memberQuery.GetMembersByEmail(club.Guid, email);
            var coach = members?.FirstOrDefault(m => m.Membership == Membership.Coach);

			if(coach == null){
				return BadRequest("Only a registered coach can provide feedback");
			} else if (model.Comments.IsEmpty()) {
				return BadRequest("Comments cannot be empty");
			}

			var response = feedbackRepository.PublishFeedback(
				new FeedbackPublishRequest {
					ClubId = club.Guid,
					PlayerId = model.PlayerId,
					Week = model.Week,
					Year = model.Year,
					Comments = model.Comments ?? "No feedback to provide",
					CoachId = coach.Guid
                }
            );
            return new JsonResult(response);
        }
    }
}
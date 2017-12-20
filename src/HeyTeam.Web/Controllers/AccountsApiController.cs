using HeyTeam.Core;
using HeyTeam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Controllers {

	[Authorize]
    [Produces("application/json")]
    [Route("api/accounts")]
    public class AccountsApiController : Controller
    {
		private readonly Club club;
		private readonly IAccountsService accountService;

		public AccountsApiController(Club club, IAccountsService accountService) {
			this.club = club;
			this.accountService = accountService;
		}

		[Authorize(Policy = "Administrator")]
		[HttpPost("invitation")]
		[ValidateAntiForgeryToken]
		public IActionResult Invitation(string email) {
			var response = accountService.SendInvitation(new InvitationRequest {
				ClubId = club.Guid,
				Email = email,
			});

			return response.RequestIsFulfilled ? Ok() : BadRequest(response.Errors) as IActionResult;
		}
	}
}
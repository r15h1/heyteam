using HeyTeam.Core;
using HeyTeam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Areas.Administration.Controllers {

    [Authorize(Policy = "Administrator")]
    [Produces("application/json")]
    [Area("Administration")]
    [Route("[area]/api/accounts")]
    public class AccountsApiController : Controller
    {
		private readonly Club club;
		private readonly IAccountsService accountService;

		public AccountsApiController(Club club, IAccountsService accountService) {
			this.club = club;
			this.accountService = accountService;
		}

		[HttpPost("invitation")]
		[ValidateAntiForgeryToken]
		public IActionResult Invitation(string email) {
			var response = accountService.SendInvitation(new AccountRequest {
				ClubId = club.Guid,
				Email = email,
			});

			return response.RequestIsFulfilled ? Ok() : BadRequest(response.Errors) as IActionResult;
		}

		[HttpPost("togglelock")]
		[ValidateAntiForgeryToken]
		public IActionResult ToggleLock(string email) {
			var response = accountService.ToggleLock(new AccountRequest {
				ClubId = club.Guid,
				Email = email,
			});

			return response.RequestIsFulfilled ? Ok() : BadRequest(response.Errors) as IActionResult;
		}
	}
}
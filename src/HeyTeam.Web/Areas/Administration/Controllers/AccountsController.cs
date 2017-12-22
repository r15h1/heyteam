using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

//https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?tabs=aspnetcore2x%2Csql-server
namespace HeyTeam.Web.Areas.Administration.Controllers
{
    [Authorize(Policy = "Administrator")]
    [Area("Administration")]
    [Route("[area]/[controller]")]
    public class AccountsController : Controller
    {
		private readonly Club club;
		private readonly IIdentityQuery identityQuery;

		public AccountsController(
			Club club, 
			IIdentityQuery identityQuery
			)
        {
			this.club = club;
			this.identityQuery = identityQuery;
		}

		[HttpGet]
		public IActionResult Index() {
			var users = identityQuery.GetUsers(club.Guid);
			var model = users?.GroupBy(u => u.Email)
								.Select(g => new UserListViewModel {
									Email = g.Key,
									Access = g.Select(u => u.Roles).Distinct(),
									Names = g.Select(u => u.Name).Distinct(),
									UserId = g.Select(u => u.UserId).FirstOrDefault(),
									AccountIsLocked = g.Select(u => u.AccountLocked).FirstOrDefault()
								}).ToList();
			return View(model);
		}		        
    }
}

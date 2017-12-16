using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HeyTeam.Web.Controllers {
	[Authorize(Policy = "Administrator")]
	[Route("[controller]")]
	public class UsersController : Controller
    {
		private readonly Club club;
		private readonly IIdentityQuery identityQuery;

		public UsersController(Club club, IIdentityQuery identityQuery) {
			this.club = club;
			this.identityQuery = identityQuery;
		}

		[HttpGet]
		public IActionResult Index()
        {
			var users = identityQuery.GetUsers(club.Guid);
			var model = users?.GroupBy(u => u.Email)
								.Select(g => new UserListViewModel {
									Email = g.Key,
									Access = g.Select(u => u.Roles).Distinct(),
									Names = g.Select(u => u.Name).Distinct(),
									UserId = g.Select(u => u.UserId).FirstOrDefault()
								}).ToList();
            return View(model);
        }
    }
}
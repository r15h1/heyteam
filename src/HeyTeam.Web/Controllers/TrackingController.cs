using HeyTeam.Core.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HeyTeam.Web.Controllers {
	[Authorize]
	[Route("[controller]/[action]")]
	public class TrackingController : Microsoft.AspNetCore.Mvc.Controller
    {
		private readonly ILibraryQuery libraryQuery;

		public TrackingController(ILibraryQuery libraryQuery) {
			this.libraryQuery= libraryQuery;
		}

        [HttpGet("{trainingMaterialId:guid}")]
		public IActionResult Library(Guid trainingMaterialId)
        {
			var trainingMaterial = libraryQuery.GetTrainingMaterial(trainingMaterialId);
			if(trainingMaterial != null) 
				return RedirectPermanent(trainingMaterial.Url);
			
            return View();
        }
    }
}
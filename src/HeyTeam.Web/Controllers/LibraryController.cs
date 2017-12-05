using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.LibraryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HeyTeam.Web.Controllers {

	[Authorize]
	[Route("[controller]")]
	public class LibraryController : Controller
    {
		private readonly Club club;
		private readonly ILibraryService libraryService;
		private readonly ILibraryQuery libraryQuery;

		public LibraryController(Club club, ILibraryService libraryService, ILibraryQuery libraryQuery) {
			this.club = club;
			this.libraryService = libraryService;
			this.libraryQuery = libraryQuery;
		}

		[HttpGet("")]
		public IActionResult Index() {
			var library = libraryQuery.GetTrainingMaterials(club.Guid);
			return View(Map(library));
		}

		[HttpGet("{trainingMaterialId:guid}")]
		public IActionResult Details(Guid trainingMaterialId) {
			var trainingMaterial = libraryQuery.GetTrainingMaterial(trainingMaterialId);
			var model = new TrainingMaterialDetailsViewModel {
				ContentType = trainingMaterial?.ContentType,
				Description = trainingMaterial?.Description,
				ExternalId = trainingMaterial?.ExternalId,
				Guid = trainingMaterial.Guid,
				ThumbnailUrl = trainingMaterial?.ThumbnailUrl,
				Title = trainingMaterial?.Title,
				Url = trainingMaterial?.Url
			};
			return View(model);
		}

		[HttpPost("{trainingMaterialId:guid}")]
		public IActionResult ReSync([FromForm]string trainingMaterialId) {
			var response = libraryService.ReSync(new ReSyncRequest { ClubId = club.Guid, TrainingMaterialId = Guid.Parse(trainingMaterialId) });
			var trainingMaterial = libraryQuery.GetTrainingMaterial(Guid.Parse(trainingMaterialId));
			var model = new TrainingMaterialDetailsViewModel {
				ContentType = trainingMaterial?.ContentType,
				Description = trainingMaterial?.Description,
				ExternalId = trainingMaterial?.ExternalId,
				ThumbnailUrl = trainingMaterial?.ThumbnailUrl,
				Title = trainingMaterial?.Title,
				Url = trainingMaterial?.Url,
				Guid = trainingMaterial?.Guid,
				Errors = !response.RequestIsFulfilled ? response.Errors : null
			};
			return View("Details", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete([FromForm]string trainingMaterialId) {
			try {
				var response = libraryService.DeleteTrainingMaterial(new TrainingMaterialDeleteRequest { ClubId = club.Guid, TrainingMaterialId = Guid.Parse(trainingMaterialId)});
				return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
		}

		public List<TrainingMaterialListViewModel> Map(IEnumerable<TrainingMaterial> trainingMaterials) {
			List<TrainingMaterialListViewModel> list = new List<TrainingMaterialListViewModel>();
			if (trainingMaterials != null)
				foreach (var m in trainingMaterials)
					list.Add(new TrainingMaterialListViewModel {
						ContentType = m.ContentType,
						Description = m.Description,
						Guid = m.Guid.Value,
						ThumbnailUrl = m.ThumbnailUrl,
						Title = m.Title,
						Url = m.Url
					});

			return list;
		}

		[HttpGet("new")]
		public IActionResult New() {
			ViewData["Title"] = "New Squad";
			ViewData["ReturnUrl"] = "/";
			return View("Create");
		}

		
		[HttpPost("new")]
		public IActionResult Create(NewTrainingMaterialViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var request = new TrainingMaterialRequest {
				ClubId = club.Guid,
				ContentType = model.ContentType,
				Description = model.Description,
				OriginalFileName = model.FileName,
				Stream = model.File.OpenReadStream(),
				Title = model.Title
			};

			var response = libraryService.AddTrainingMaterial(request);
			if(!response.RequestIsFulfilled) {
				foreach (var error in response.Errors) {
					ModelState.AddModelError("", error);
					return View(model);
				}
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
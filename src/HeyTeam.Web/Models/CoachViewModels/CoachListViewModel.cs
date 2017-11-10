using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.CoachViewModels {
	public class AssignCoachViewModel {
		public Guid SquadId { get; set; }

		[Required(ErrorMessage = "Please select a coach")]
		public Guid? SelectedCoach { get; set; }
		public string SquadName { get; set; }
		public List<SelectListItem> Coaches { get; set; }
	}
}

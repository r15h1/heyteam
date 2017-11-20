using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels {
	public class NewEventViewModel {
		public List<SelectListItem> SquadList { get; set; } = new List<SelectListItem>();

		[Required]
		[StringLength(100)]
		public string Title { get; set; }
		
		[Required]
		[MinLength(1)]
		public Guid[] Squads { get; set; }

		[Required]
		[FutureDate(ErrorMessage = "Start Date must be in the future")]
		[Display(Name = "Start Date and Time")]
		public DateTime? StartDate { get; set; } 
		
		[Required]
		[FutureDate(ErrorMessage = "End Date and Time must be greater than Start Date and be in the future")]
		[Display(Name = "End Date and Time")]
		public DateTime? EndDate { get; set; }

		[Required]
		[StringLength(400)]
		public string Location { get; set; }
		
	}

	public class FutureDateAttribute : ValidationAttribute {
		public override bool IsValid(object value) {
			DateTime d = Convert.ToDateTime(value);
			return d > DateTime.Now;
		}
	}
}
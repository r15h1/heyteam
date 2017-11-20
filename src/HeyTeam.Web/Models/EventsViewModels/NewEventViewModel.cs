using HeyTeam.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

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
		[Display(Name = "Start Date and Time")]
		public DateTime? StartDate { get; set; } 
		
		[Required]
		[Display(Name = "End Date and Time")]
		public DateTime? EndDate { get; set; }

		[Required]
		[StringLength(250)]
		public string Street { get; set; }

		[Required]
		[StringLength(50)]
		public string City { get; set; }

		[Required]
		[StringLength(7)]
		[Display(Name = "Postal Code / Zip")]
		public string PostalCode { get; set; }

		[Required]
		[StringLength(25)]
		public string Country { get; set; }
	}

	public class RequiredSquadAttribute : ValidationAttribute {
		public override bool IsValid(object value) {
			return false;
		}
	}
}
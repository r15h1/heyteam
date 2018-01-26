using HeyTeam.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.AvailabilityViewModels
{
    public class NewAvailabilityViewModel
    {
		[Required]
		public Guid PlayerId { get; set; }

		[Required]
		public DateTime DateFrom { get; set; }

		[Required]
		public AvailabilityStatus? AvailabilityStatus { get; set; }

		public DateTime? DateTo { get; set; }
		
		public string Notes { get; set; }
	}
}

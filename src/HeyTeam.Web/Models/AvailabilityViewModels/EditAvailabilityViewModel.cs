using HeyTeam.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.AvailabilityViewModels {
	public class EditAvailabilityViewModel
    {
        [Required]
        public Guid AvailabilityId { get; set; }

        [Required]
		public Guid PlayerId { get; set; }

		[Required]
		public DateTime? DateFrom { get; set; }

		[Required]
		public AvailabilityStatus? AvailabilityStatus { get; set; }

		public DateTime? DateTo { get; set; }
		
		public string Notes { get; set; }

        public string SelectedPlayer { get; set; }
	}
}

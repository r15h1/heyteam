using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.TrackingViewModels {
	public class LibraryTrackingViewModel
    {
        [Required]
        public Guid TrainingMaterialId { get; set; }

        [Required]
        public Guid MemberId { get; set; }

        [Required]//event, assignment etc
        public Guid SourceId { get; set; }
    }
}

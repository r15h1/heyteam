using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.TrackingViewModels
{
    public class LibraryTrackingViewModel
    {
        [Required]
        public Guid TrainingMaterialId { get; set; }

        [Required]
        public Guid MemberId { get; set; }

        [Required]
        public Guid EventId { get; set; }
    }
}

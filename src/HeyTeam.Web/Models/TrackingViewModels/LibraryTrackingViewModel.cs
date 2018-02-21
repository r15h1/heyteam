using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.TrackingViewModels
{
    public class LibraryTrackingViewModel
    {
        public Guid TrainingMaterialId { get; set; }
        public Guid MemberId { get; set; }
        public Guid EventId { get; set; }
    }
}

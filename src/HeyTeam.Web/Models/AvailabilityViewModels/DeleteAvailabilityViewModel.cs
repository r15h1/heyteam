using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.AvailabilityViewModels
{
    public class DeleteAvailabilityViewModel
    {
        public Guid PlayerId { get; set; }
        public Guid AvailabilityId { get; set; }
    }
}

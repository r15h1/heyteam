using System;

namespace HeyTeam.Core.Models
{
    public class DeleteAvailabilityRequest
    {
        public Guid ClubId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid AvailabilityId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Services
{
    public interface ITracker
    {
        Response Track(TrackLibraryRequest request);
    }

    public class TrackLibraryRequest
    {
        public Guid MemberId { get; set; }
        public Membership? Membership { get; set; } 
        public Guid EventId { get; set; }
        public Guid TrainingMaterialId { get; set; }
    }
}

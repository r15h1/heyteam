using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Services
{
    public interface ITracker
    {
        Response Track(EventTrainingMaterialViewRequest request);
    }

    public class EventTrainingMaterialViewRequest
    {
        public Guid ClubId { get; set; }
        public Guid MemberId { get; set; }
        public Membership? Membership { get; set; } 
        public Guid EventId { get; set; }
        public Guid TrainingMaterialId { get; set; }
    }
}

using HeyTeam.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Repositories
{
    public interface IFeedbackRepository
    {
        Response PublishFeedback(FeedbackPublishRequest request);
    }

    public class FeedbackPublishRequest
    {
        public Guid ClubId { get; set; }
        public Guid PlayerId { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public Guid CoachId { get; set; }
        public string Comments { get; set; }
    }
}

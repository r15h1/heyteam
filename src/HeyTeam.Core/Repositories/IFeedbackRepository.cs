using HeyTeam.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Repositories
{
    public interface IFeedbackRepository
    {
        Response PublishFeedback(FeedbackPublishRequest request);
        Response AddComment(AddCommentRequest request);
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

    public class AddCommentRequest
    {
        public Guid ClubId { get; set; }       
        public string Comment { get; set; }
        public Guid FeedbackId { get; set; }
        public string PostedBy { get; set; }
        public Guid PosterId { get; set; }
    }
}

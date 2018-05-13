using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Queries
{
    public interface IFeedbackQuery
    {
        IEnumerable<MiniFeedback> GetFeedbackList(FeedbackListRequest request);
        MiniFeedbackChain GetFeedbackChain(FeedbackChainRequest request);
    }

    public class FeedbackListRequest
    {
        public Guid ClubId { get; set; }
        public Guid SquadId { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
    }

    public class FeedbackChainRequest
    {
        public Guid ClubId { get; set; }
        public Guid FeedbackId { get; set; }
    }
}

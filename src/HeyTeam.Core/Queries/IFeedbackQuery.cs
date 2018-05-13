using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Queries
{
    public interface IFeedbackQuery
    {
        IEnumerable<MiniFeedback> GetFeedback(FeedbackRequest request);
    }

    public class FeedbackRequest
    {
        public Guid ClubId { get; set; }
        public Guid SquadId { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
    }
}

using System;

namespace HeyTeam.Web.Models.EvaluationViewModels
{
    public class TermInfoModel
    {
        public Guid? TermId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

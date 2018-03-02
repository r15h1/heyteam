using System;
using System.Collections.Generic;

namespace HeyTeam.Web.Models.EvaluationViewModels
{
    public class ReportCardDesignListViewModel
    {
        public TermInfoModel TermInfo { get; set; }
        public List<MiniReportCardViewModel> ReportCardDesigns { get; set; } = new List<MiniReportCardViewModel>();
    }

    public class MiniReportCardViewModel
    {
        public Guid ReportCardDesignId { get; set; }
        public string Name { get; set; }
    }
}

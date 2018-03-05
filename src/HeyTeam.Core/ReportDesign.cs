using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core
{
    public class ReportDesign
    {
        public ReportDesign(Guid ClubId, Guid? reportCardId = null)
        {
            this.ClubId = ClubId;
            Guid = reportCardId ?? System.Guid.NewGuid();
        }

        public Guid ClubId { get; }
        public Guid? Guid { get; }
        public string Name { get; set; }
        public ICollection<ReportHeadline> ReportHeadlines { get; } = new List<ReportHeadline>();

        public void AddReportHeadline(ReportHeadline headline)
        {
            if (headline != null && !ReportHeadlines.Any(h => h.Guid == headline.Guid))
                ReportHeadlines.Add(headline);
        }
    }
}
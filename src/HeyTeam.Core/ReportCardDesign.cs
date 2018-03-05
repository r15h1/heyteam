using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core
{
    public class ReportCardDesign
    {
        public ReportCardDesign(Guid ClubId, Guid? reportCardId = null)
        {
            this.ClubId = ClubId;
            Guid = reportCardId ?? System.Guid.NewGuid();
        }

        public Guid ClubId { get; }
        public Guid? Guid { get; }
        public string Name { get; set; }
        public ICollection<ReportCardHeadline> ReportHeadlines { get; } = new List<ReportCardHeadline>();

        public void AddReportHeadline(ReportCardHeadline headline)
        {
            if (headline != null && !ReportHeadlines.Any(h => h.Guid == headline.Guid))
                ReportHeadlines.Add(headline);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core
{
    public class ReportCardHeadline
    {
        public ReportCardHeadline(Guid reportDesignGuid, Guid? reportCardHeadlineId)
        {
            ReportDesignGuid = reportDesignGuid;
            Guid = reportCardHeadlineId ?? System.Guid.NewGuid();
        }

        public Guid ReportDesignGuid { get; }
        public Guid? Guid { get; }
        public string Title { get; set; }
        public short SortOrder { get; set; }
        public ICollection<ReportCardArea> ReportAreas{ get; } = new List<ReportCardArea>();

        public void AddReportArea(ReportCardArea reportArea)
        {
            if (reportArea != null && !ReportAreas.Any(h => h.Guid == reportArea.Guid))
                ReportAreas.Add(reportArea);
        }
    }
}
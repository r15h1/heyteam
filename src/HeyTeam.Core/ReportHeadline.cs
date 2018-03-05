using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core
{
    public class ReportHeadline
    {
        public ReportHeadline(Guid reportDesignGuid, Guid? reportCardHeadlineId)
        {
            ReportDesignGuid = reportDesignGuid;
            Guid = reportCardHeadlineId ?? System.Guid.NewGuid();
        }

        public Guid ReportDesignGuid { get; }
        public Guid? Guid { get; }
        public string Title { get; set; }
        public short SortOrder { get; set; }
        public ICollection<ReportArea> ReportAreas{ get; } = new List<ReportArea>();

        public void AddReportArea(ReportArea reportArea)
        {
            if (reportArea != null && !ReportAreas.Any(h => h.Guid == reportArea.Guid))
                ReportAreas.Add(reportArea);
        }
    }
}
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries
{
    public interface IReportDesignerQuery
    {
        IEnumerable<ReportDesign> GetReportCardDesigns(Guid clubId, string name = null);
        ReportDesign GetReportCardDesign(Guid reportDesignId);
    }
}

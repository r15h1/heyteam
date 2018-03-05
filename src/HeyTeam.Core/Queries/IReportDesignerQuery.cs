using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries
{
    public interface IReportDesignerQuery
    {
        IEnumerable<ReportCardDesign> GetReportCardDesigns(Guid clubId, string name = null);
        ReportCardDesign GetReportCardDesign(Guid reportDesignId);
    }
}

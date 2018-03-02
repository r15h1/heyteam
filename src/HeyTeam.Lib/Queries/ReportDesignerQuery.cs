using System;
using System.Collections.Generic;
using HeyTeam.Core;
using HeyTeam.Core.Queries;

namespace HeyTeam.Lib.Queries
{
    public class ReportDesignerQuery : IReportDesignerQuery
    {
        public IEnumerable<ReportDesign> GetReportCardDesigns(Guid clubId, string name = null)
        {
            return new List<ReportDesign>();
        }
    }
}

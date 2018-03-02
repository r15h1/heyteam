using HeyTeam.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Services
{
    public class ReportDesigner : IReportDesigner
    {
        public (Guid Guid, Response Response) CreateReportCardDesign(NewReportCardDesignRequest request)
        {
            return (Guid.Empty, Response.CreateResponse(new NotImplementedException()));
        }
    }
}

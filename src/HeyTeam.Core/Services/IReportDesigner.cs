using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Services
{
    public interface IReportDesigner
    {
        (Guid Guid, Response Response) CreateReportCardDesign(NewReportDesignRequest request);

    }

    public class NewReportDesignRequest
    {
        public Guid ClubId { get; set; }
        public string Name { get; set; }
    }
}

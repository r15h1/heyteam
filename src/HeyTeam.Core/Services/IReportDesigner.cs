using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Services
{
    public interface IReportDesigner
    {
        (Guid Guid, Response Response) CreateReportCardDesign(NewReportCardDesignRequest request);

    }

    public class NewReportCardDesignRequest
    {
        public Guid ClubId { get; set; }
        public Guid TermId { get; set; }
        public string Name { get; set; }
    }
}

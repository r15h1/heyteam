using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Models
{
    public class ReportCard
    {
        public Guid Guid { get; set; }
        public MiniModel Design { get; set; }
        public ICollection<MiniReportCardHeadline> Headlines { get; set; }
    }
}

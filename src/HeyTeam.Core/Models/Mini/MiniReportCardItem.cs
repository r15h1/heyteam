using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Models.Mini
{
    public class MiniReportCardItem 
    {
        public Guid Guid { get; set; }
        public Guid Parent { get; set; }
        public string Title { get; set; }
        public short SortOrder { get; set; }
    }

    public class MiniReportCardHeadline : MiniReportCardItem
    {
        public ICollection<MiniReportCardItem> Areas { get; set; }
    }

    public class MiniReportCardArea : MiniReportCardItem
    {
        public ICollection<MiniReportCardItem> Skills { get; set; }
    }

    public class MiniReportCardSkill : MiniReportCardItem
    {
        public ReportCardGrade Grade { get; set; }
    }
}

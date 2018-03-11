using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Util;

namespace HeyTeam.Core.Models.Mini
{
    public class MiniReportCardItem 
    {
        public MiniReportCardItem(Guid itemId)
        {
            Guid = itemId;
        }
        public Guid Guid { get; }        
        public string Title { get; set; }
        public short SortOrder { get; set; }
    }

    public class MiniReportCardHeadline : MiniReportCardItem
    {
        public MiniReportCardHeadline(Guid itemId) : base(itemId){}
        public ICollection<MiniReportCardArea> Areas { get; } = new List<MiniReportCardArea>();
        public void AddArea(MiniReportCardArea area)
        {
            if(area !=null && !area.Guid.IsEmpty() && !Areas.Any(a => a.Guid == area.Guid))
            {
                Areas.Add(area);
            }
        }
    }

    public class MiniReportCardArea : MiniReportCardItem
    {
        public MiniReportCardArea(Guid itemId, Guid headlineId) : base(itemId) {
            HeadlineId = headlineId;
        }

        public Guid HeadlineId { get; }
        public ICollection<MiniReportCardItem> Skills { get; } = new List<MiniReportCardItem>();
        public void AddSkill(MiniReportCardItem skill)
        {
            if (skill != null && !skill.Guid.IsEmpty() && !Skills.Any(s => s.Guid == skill.Guid))
            {
                Skills.Add(skill);
            }
        }
    }

    public class MiniReportCardSkill : MiniReportCardItem
    {
        public MiniReportCardSkill(Guid itemId, Guid areaId) : base(itemId)
        {
            AreaId = areaId;
        }

        public Guid AreaId { get; }
        public ReportCardGrade? Grade { get; set; }
    }
}

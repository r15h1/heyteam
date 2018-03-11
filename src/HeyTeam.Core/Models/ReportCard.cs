using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Util;

namespace HeyTeam.Core.Models
{
    public class ReportCard
    {
        public ReportCard(Guid reportCardId)
        {
            Guid = reportCardId;
        }
        public Guid Guid { get; }
        public MiniModel Design { get; set; }
        public ICollection<MiniReportCardHeadline> Headlines { get; } = new List<MiniReportCardHeadline>();
        public void AddHeadline(MiniReportCardHeadline headline)
        {
            if (headline != null && !headline.Guid.IsEmpty() && !Headlines.Any(h => h.Guid == headline.Guid))
            {
                Headlines.Add(headline);
            }
        }

        public void AddArea(MiniReportCardArea area)
        {
            if (area != null && !area.Guid.IsEmpty() && !area.HeadlineId.IsEmpty())
            {
                var targetHeadline = Headlines.SingleOrDefault(h => h.Guid == area.HeadlineId);
                if (targetHeadline != null)
                {
                    targetHeadline.AddArea(area);
                }
            }
        }

        public void AddSkill(MiniReportCardSkill skill)
        {
            if (skill != null && !skill.Guid.IsEmpty() && !skill.AreaId.IsEmpty())
            {
                var targetArea = Headlines.SelectMany(h => h.Areas).SingleOrDefault(a => a.Guid == skill.AreaId);
                targetArea.AddSkill(skill);
            }
        }
    }
}

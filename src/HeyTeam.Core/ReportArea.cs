using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core
{
    public class ReportArea
    {
        public ReportArea(Guid reportCardHeadlineId, Guid? reportAreaId = null)
        {
            this.ReportCardHeadlineId = reportCardHeadlineId;
            Guid = reportAreaId ?? System.Guid.NewGuid();
        }

        public Guid ReportCardHeadlineId { get; }
        public Guid? Guid { get; }
        public string Title { get; set; }
        public short SortOrder { get; set; }
        public ICollection<ReportSkill> ReportSkills { get; } = new List<ReportSkill>();

        public void AddReportSkill(ReportSkill skill)
        {
            if (skill != null && !ReportSkills.Any(h => h.Guid == skill.Guid))
                ReportSkills.Add(skill);
        }
    }
}

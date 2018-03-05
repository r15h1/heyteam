using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core
{
    public class ReportCardArea
    {
        public ReportCardArea(Guid reportCardHeadlineId, Guid? reportAreaId = null)
        {
            this.ReportCardHeadlineId = reportCardHeadlineId;
            Guid = reportAreaId ?? System.Guid.NewGuid();
        }

        public Guid ReportCardHeadlineId { get; }
        public Guid? Guid { get; }
        public string Title { get; set; }
        public short SortOrder { get; set; }
        public ICollection<ReportCardSkill> ReportSkills { get; } = new List<ReportCardSkill>();

        public void AddReportSkill(ReportCardSkill skill)
        {
            if (skill != null && !ReportSkills.Any(h => h.Guid == skill.Guid))
                ReportSkills.Add(skill);
        }
    }
}

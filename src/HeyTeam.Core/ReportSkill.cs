using System;

namespace HeyTeam.Core
{
    public class ReportSkill
    {
        public ReportSkill(Guid reportAreaId, Guid? reportSkillId = null)
        {
            this.ReportCardHeadlineId = reportAreaId;
            Guid = reportSkillId ?? System.Guid.NewGuid();
        }

        public Guid ReportCardHeadlineId { get; }
        public Guid? Guid { get; }
        public string Title { get; set; }
        public short SortOrder { get; set; }
    }
}

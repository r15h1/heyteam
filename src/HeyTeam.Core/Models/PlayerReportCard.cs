using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core.Models
{
    public class PlayerReportCard
    {
        public PlayerReportCard(Guid playerId, Guid termId, Guid reportCardDesignId, Guid? playerReportCardId = null)
        {
            PlayerId = playerId;
            TermId = termId;
            ReportCardDesignId = reportCardDesignId;
            PlayerReportCardId = playerReportCardId ?? Guid.NewGuid();
        }

        public Guid PlayerId { get; }
        public Guid TermId { get; }
        public Guid ReportCardDesignId { get; }
        public Guid PlayerReportCardId { get; }        
    }

	public class PlayerReportCardWithGrades : PlayerReportCard {
		public PlayerReportCardWithGrades(Guid playerId, Guid termId, Guid reportCardDesignId, Guid? playerReportCardId = null) 
			: base(playerId, termId, reportCardDesignId, playerReportCardId) {
		}

		public ICollection<PlayerReportCardGrade> PlayerReportCardGrades { get; } = new List<PlayerReportCardGrade>();
		public void AddReportCardGrade(Guid reportCardSkillId, ReportCardGrade grade) {
			if (PlayerReportCardGrades.Any(g => g.ReportCardSkillId == reportCardSkillId)) {
				PlayerReportCardGrades.Where(g => g.ReportCardSkillId == reportCardSkillId).Single().ReportCardGrade = grade;
			} else {
				PlayerReportCardGrades.Add(new PlayerReportCardGrade(PlayerReportCardId, reportCardSkillId) { ReportCardGrade = grade });
			}
		}
	}

    public class PlayerReportCardGrade
    {
        public PlayerReportCardGrade(Guid playerReportCardId, Guid reportCardSkillId)
        {
            PlayerReportCardId = playerReportCardId;
            ReportCardSkillId = reportCardSkillId;
        }

        public Guid PlayerReportCardId { get; }
        public Guid ReportCardSkillId { get; }
        public ReportCardGrade? ReportCardGrade { get; set; }
    }
}
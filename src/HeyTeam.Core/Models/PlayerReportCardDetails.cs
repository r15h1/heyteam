using System;

namespace HeyTeam.Core.Models
{
    public class PlayerReportCardDetails : PlayerReportCard
    {
        public PlayerReportCardDetails(Guid clubId, Guid playerId, Guid playerReportCardId, Guid reportCardDesignId) : base(playerId, playerReportCardId)
        {
            ReportCard = new ReportCardDesign(clubId, reportCardDesignId);
        }

        public ReportCardDesign ReportCard { get; private set; }
        public Term Term { get; set; }
    }
}

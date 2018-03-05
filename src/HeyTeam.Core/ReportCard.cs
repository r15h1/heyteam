using System;

namespace HeyTeam.Core
{
    public class ReportCard
    {
        public ReportCard(Guid ClubId, Guid playerId, Guid? reportCardId = null) 
        {
            this.ClubId = ClubId;
            PlayerId = playerId;
            ReportCardId = reportCardId ?? Guid.NewGuid();
        }

        public Guid ClubId { get; }
        public Guid PlayerId { get; }
        public Guid ReportCardId { get; }
    }
}

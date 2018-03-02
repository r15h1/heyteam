using System;

namespace HeyTeam.Core
{
    public class ReportDesign
    {
        public ReportDesign(Guid ClubId, Guid? reportCardId = null)
        {
            this.ClubId = ClubId;
            Guid = reportCardId ?? System.Guid.NewGuid();
        }

        public Guid ClubId { get; }
        public Guid? Guid { get; }
        public string Name { get; set; }
    }
}
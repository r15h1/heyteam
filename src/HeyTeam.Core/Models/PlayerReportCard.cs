using HeyTeam.Util;
using System;

namespace HeyTeam.Core.Models {
	public class PlayerReportCard
    {
        public PlayerReportCard(Guid playerId, Guid? playerReportCardId = null)
        {
            PlayerId = playerId;
            PlayerReportCardId = playerReportCardId;
        }

        public Guid PlayerId { get; }
		public string PlayerName { get; set; }
		public short? SquadNumber { get; set; }		
        public Guid? PlayerReportCardId { get; } 
		public bool ReportCardExists{ get => !PlayerReportCardId.IsEmpty(); }
    }	
}
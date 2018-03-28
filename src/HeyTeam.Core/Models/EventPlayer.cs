using HeyTeam.Util;
using System;

namespace HeyTeam.Core.Models {
	public class EventPlayer
    {

		public EventPlayer(Guid squadGuid, Guid eventGuid, Guid playerGuid) {
			if (squadGuid.IsEmpty() || eventGuid.IsEmpty() || playerGuid.IsEmpty())
				throw new ArgumentNullException("guid");

			SquadGuid = squadGuid;
			EventGuid = eventGuid;
			PlayerGuid = playerGuid;
		}

		public Guid SquadGuid { get; }
		public Guid EventGuid { get; }
		public Guid PlayerGuid { get; }
		public string SquadName { get; set; }
		public string PlayerName { get; set; }
		public short SquadNumber { get; set; }
		public Attendance? Attendance { get; set; }
		public short? TimeLogged{ get; set; }
		public string Feedback { get; set; }
	}
}

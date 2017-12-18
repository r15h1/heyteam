using HeyTeam.Util;
using System;

namespace HeyTeam.Core {
	public class Coach : Member {
		public Coach(Guid clubId, Guid? coachId = null) : base(coachId) {
			if (clubId.IsEmpty()) throw new ArgumentNullException();				
			ClubId = clubId;			
		}

		public Guid ClubId { get; }				
		public string Qualifications{ get; set; }
	}
}
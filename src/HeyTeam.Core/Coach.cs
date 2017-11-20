using HeyTeam.Util;
using System;

namespace HeyTeam.Core {
	public class Coach {
		public Coach(Guid clubId, Guid? coachId = null) {
			if (clubId.IsEmpty()) throw new ArgumentNullException();
				
			ClubId = clubId;
			Guid = coachId.HasValue && coachId.Value != Guid.Empty ? coachId.Value : Guid.NewGuid();
		}

		public Guid ClubId { get; }
		public Guid Guid { get; }
		public DateTime DateOfBirth { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Qualifications{ get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core
{
    public class Member {

		public Member(Guid? guid = null) {
			Guid = guid.HasValue && guid.Value != Guid.Empty ? guid.Value : Guid.NewGuid();
		}

		public Guid Guid { get; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public DateTime DateOfBirth { get; set; }
		public Membership Membership{ get; set; }
	}

	public enum Membership {
		Player,
		Coach
	}
}

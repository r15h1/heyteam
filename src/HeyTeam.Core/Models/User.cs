using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models
{
    public class User
    {
		public User(string userId) {
			UserId = userId;
		}

		public string Name { get; set; }
		public string Email { get; set; }
		public string Roles { get; set; }
		public string UserId { get; }
		public bool? AccountLocked { get; set; }
		public bool? EmailConfirmed { get; set; }
	}
}

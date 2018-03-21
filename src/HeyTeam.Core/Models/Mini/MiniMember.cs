using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
	public class MiniMember : MiniModel {
		public MiniMember(Guid guid, string name) : base(guid, name) {
		}

		public string Membership{ get; set; }
	}
}

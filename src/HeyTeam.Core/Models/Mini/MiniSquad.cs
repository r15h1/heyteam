using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
	public class MiniSquad : MiniModel {
		public MiniSquad(Guid guid, string name) : base(guid, name) {
		}

		public IEnumerable<MiniMember> Members { get; } = new List<MiniMember>();
		public void AddMember(MiniMember member){
			if (!Members.Any(m => m.Guid == member.Guid))
				((ICollection<MiniMember>)Members).Add(member);
		}
	}
}
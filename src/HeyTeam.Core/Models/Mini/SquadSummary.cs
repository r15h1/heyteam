using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models.Mini
{
	public class SquadSummary : MiniModel {
		public SquadSummary(Guid guid, string name) : base(guid, name) {
		}

		public int NumberOfPlayers{ get; set; }
		public short YearBorn { get; set; }
		public MiniEvent NextEvent{ get; set; }
	}
}

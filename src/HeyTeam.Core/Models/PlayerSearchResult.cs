using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models
{
    public class PlayerSearchResult
    {
		public string PlayerName { get; set; }
		public Guid PlayerId { get; set; }
		public string SquadName { get; set; }
		public string SquadNumber { get; set; }
	}
}

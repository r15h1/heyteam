using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Services {
	public interface IPlayerService
    {
		Response RegisterPlayer(PlayerRequest request);
		Response UpdatePlayerProfile(PlayerRequest request);
	}

	public class PlayerRequest {
		public Guid SquadId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public char DominantFoot { get; set; }
		public string Nationality { get; set; }
		public short? SquadNumber { get; set; }
		public List<string> Positions { get; set; } = new List<string>();
		public DateTime? DateOfBirth { get; set; }
		public string Email { get; set; }
		public Guid? PlayerId { get; set; }
	}
}
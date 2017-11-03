using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.PlayerViewModels
{
    public class PlayerDetailsViewModel
    {
		public string SquadName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string DominantFoot { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Email { get; set; }
		public short? SquadNumber { get; set; }
		public string Nationality { get; set; }
	}
}

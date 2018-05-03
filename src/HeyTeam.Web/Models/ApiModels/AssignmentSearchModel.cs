using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.ApiModels
{
    public class AssignmentSearchModel
    {
		public IEnumerable<Guid> Players { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public DateTime? Date{ get; set; }
	}
}

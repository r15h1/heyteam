using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.ApiModels
{
    public class AssignmentSearchModel
    {
		public IEnumerable<Guid> Squads { get; set; }
		public int Month { get; set; } = DateTime.Today.Month;
		public int Year { get; set; } = DateTime.Today.Year;
	}
}

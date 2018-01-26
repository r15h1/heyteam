using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Queries
{
    public interface IAvailabilityQuery
    {
		IEnumerable<Availability> GetAvailabilities(AvailabilityRequest request);
	}

	public class AvailabilityRequest{
		public Guid ClubId{ get; set; }
		public Guid? SquadId { get; set; }
		public Guid? PlayerId { get; set; }		
	}
}

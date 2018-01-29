using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Queries
{
    public interface IAvailabilityQuery
    {
		IEnumerable<Availability> GetAvailabilities(GetAvailabilityRequest request);
	}

	public class GetAvailabilityRequest{
		public Guid ClubId{ get; set; }
		public Guid? SquadId { get; set; }
		public Guid? PlayerId { get; set; }		
	}
}

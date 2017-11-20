using HeyTeam.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Services
{
    public interface ISquadService
    {
		Response RegisterSquad(SquadRequest squad);
		Response UpdateSquadProfile(SquadRequest squad);
		Response AssignCoach(Guid squadId, Guid coachId);
		Response UnAssignCoach(Guid squadId, Guid coachId);
	}

	public class SquadRequest {
		public Guid ClubId { get; set; }
		public Guid? SquadId { get; set; }
		public string SquadName { get; set; }
	}
}

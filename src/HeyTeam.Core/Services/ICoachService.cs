using HeyTeam.Core.Entities;
using System;

namespace HeyTeam.Core.Services {
	public interface ICoachService
    {
		Response RegisterCoach(CoachRequest request);
		Response UpdateCoachProfile(CoachRequest request);
	}

	public class CoachRequest {
		public Guid? CoachId { get; set; }
		public Guid ClubId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Qualifications { get; set; }
		public DateTime DateOfBirth { get; set; }
	}
}

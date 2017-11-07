using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.UseCases.Coach {
	public class SaveCoachRequest {
		public Guid? CoachId { get; set; }
		public Guid ClubId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Qualifications { get; set; }
		public DateTime DateOfBirth { get; set; }
		public Action Command { get; set; } = Action.ADD;

		public enum Action {
			ADD,
			UPDATE
		}
	}
}

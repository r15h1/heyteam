﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.UseCases.Coach {
	public class AddCoachRequest {
		public Guid ClubId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Qualifications { get; set; }
		public DateTime DateOfBirth { get; set; }
	}
}

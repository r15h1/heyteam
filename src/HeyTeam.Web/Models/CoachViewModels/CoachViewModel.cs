using HeyTeam.Web.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.CoachViewModels {
	public class CoachViewModel
    {
		public Guid? CoachId { get; set; }

		[Required]
		public Guid ClubId { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[MinimumAge(15, ErrorMessage = "Coach must be at least 15 years old")]
		[Display(Name = "Date of Birth")]
		public DateTime DateOfBirth { get; set; }

		[Required]
		[EmailAddress]
		[MaxLength(320)]
		public string Email { get; set; }

		[Required]
		[Phone]
		[MinLength(7)]
		[MaxLength(20)]
		public string Phone { get; set; }

		[Display(Name = "Qualifications")]
		public string Qualifications { get; set; }
	}
}

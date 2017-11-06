using System;
using System.ComponentModel.DataAnnotations;
using HeyTeam.Util;

namespace HeyTeam.Web.Models.PlayerViewModels {
    public class PlayerViewModel {
		public Guid? PlayerId { get; set; }

		[Required]
		public Guid SquadId { get; set; }

		public string SquadName { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[MaxLength(1)]
		[DominantFoot(ErrorMessage = "R for Right, L for Left")]
		[Display(Name = "Dominant Foot")]		
		public string DominantFoot { get; set; }

		[Required]
		[PlayerDateOfBirth(ErrorMessage = "Player must be at least 4 years old at registration time")]
		[Display(Name = "Date of Birth")]
		public DateTime DateOfBirth { get; set; }

		[Required]
		[EmailAddress]
		[MaxLength(320)]
		public string Email { get; set; }

		[Display(Name = "Squad#")]
		public short? SquadNumber { get; set; }

		[Required]
		[MaxLength(50)]
		public string Nationality { get; set; }
	}

	public class PlayerDateOfBirthAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			DateTime d = Convert.ToDateTime(value);
			return d <= DateTime.Now.AddYears(-4);

		}
	}

	public class DominantFootAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			string foot = value.ToString().ToUpper();
			return !foot.IsEmpty() && (foot.Equals("R") || foot.Equals("L"));
		}
	}
}
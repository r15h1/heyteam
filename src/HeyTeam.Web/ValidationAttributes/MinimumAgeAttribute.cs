using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.ValidationAttributes {
	public class MinimumAgeAttribute : ValidationAttribute {
		public MinimumAgeAttribute(int minimumAge) {
			MinimumAge = minimumAge;
		}

		public int MinimumAge { get; }

		public override bool IsValid(object value) {
			DateTime d = Convert.ToDateTime(value);
			return d.Date <= DateTime.Today.AddYears(-MinimumAge);
		}
	}
}
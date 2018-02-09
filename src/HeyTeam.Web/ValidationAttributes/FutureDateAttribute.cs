using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.ValidationAttributes {
	public class FutureDateAttribute : ValidationAttribute {
		
		public FutureDateAttribute(bool ignoreTime = false) {
			IgnoreTime = ignoreTime;
		}

		public bool IgnoreTime { get; }

		public override bool IsValid(object value) {
			DateTime d = Convert.ToDateTime(value);
			return IgnoreTime ? d.Date > DateTime.Today : d > DateTime.Now;
		}
	}
}
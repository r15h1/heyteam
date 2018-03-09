using HeyTeam.Util;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.ValidationAttributes {
	public class NotEmptyAttribute : ValidationAttribute {
		public NotEmptyAttribute() {}			

		public override bool IsValid(object value) {
			return !((Guid)value).IsEmpty();
		}
	}
}
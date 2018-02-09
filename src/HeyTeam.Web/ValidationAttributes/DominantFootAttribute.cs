using HeyTeam.Util;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HeyTeam.Web.ValidationAttributes {
	public class DominantFootAttribute : ValidationAttribute {
		public override bool IsValid(object value) {
			string foot = value.ToString().ToUpper();
			return !foot.IsEmpty() && (foot.Equals("R") || foot.Equals("L"));
		}
	}
}
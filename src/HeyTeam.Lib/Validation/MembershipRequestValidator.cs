using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
	public class MembershipRequestValidator : IValidator<MembershipRequest> {
		public ValidationResult<MembershipRequest> Validate(MembershipRequest request) {
			var result = new ValidationResult<MembershipRequest>(request);
			if (request == null) {
				result.AddMessage("Request cannot be null");
			} else {
				if (request.ClubId.IsEmpty())
					result.AddMessage("ClubId cannot be null");

				if (!request.Email.IsValidEmail())
					result.AddMessage("Email is not valid");

				if (request.Password.IsEmpty() || request.Password.Trim().Length < 8)
					result.AddMessage("Password is too short");
			}
			return result;
		}
	}
}

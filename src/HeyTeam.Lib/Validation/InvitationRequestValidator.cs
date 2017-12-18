using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using System;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation
{
	public class InvitationRequestValidator : IValidator<InvitationRequest> {
		public ValidationResult<InvitationRequest> Validate(InvitationRequest request) {
			var result = new ValidationResult<InvitationRequest>(request);
			if (request == null) {
				result.AddMessage("Request cannot be null");
			} else {
				if (request.ClubId.IsEmpty())
					result.AddMessage("ClubId cannot be null");

				if (!request.Email.IsValidEmail())
					result.AddMessage("Email is not valid");
			}
			return result;
		}
	}
}

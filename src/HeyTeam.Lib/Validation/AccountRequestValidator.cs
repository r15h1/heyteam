using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using System;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation
{
	public class AccountRequestValidator : IValidator<AccountRequest> {
		public ValidationResult<AccountRequest> Validate(AccountRequest request) {
			var result = new ValidationResult<AccountRequest>(request);
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

using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Validation {
	public class NewAvailabilityRequestValidator : IValidator<NewAvailabilityRequest> {
		public ValidationResult<NewAvailabilityRequest> Validate(NewAvailabilityRequest request) {
			var validationResult = new ValidationResult<NewAvailabilityRequest>(request);
			if (request == null) {
				validationResult.AddMessage("Request cannot be null");
				return validationResult;
			}

			if (request.ClubId.IsEmpty())
				validationResult.AddMessage("ClubId cannot be empty");

			if (request.PlayerId.IsEmpty())
				validationResult.AddMessage("PlayerId cannot be empty");

			if (request.DateFrom <= DateTime.Today.AddDays(-30) || request.DateFrom >= DateTime.Today.AddMonths(3))
				validationResult.AddMessage($"Date From must be between {DateTime.Today.AddDays(-30).ToString("dd MMM yyyy")} and {DateTime.Today.AddMonths(3).ToString("dd MMM yyyy")}");

			if (request.DateTo.HasValue && (request.DateTo.Value < request.DateFrom ))
				validationResult.AddMessage("DateTo must be greater or equal to DateFrom");			

			return validationResult;
		}
	}
}

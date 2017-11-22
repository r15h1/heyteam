using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Validation
{
	public class EventDeleteRequestValidator : IValidator<EventDeleteRequest> {
		public ValidationResult<EventDeleteRequest> Validate(EventDeleteRequest request) {
			var result = new ValidationResult<EventDeleteRequest>(request);
			if (request == null) {
				result.AddMessage("Request cannot be null");
				return result;
			}

			if(request.ClubId.IsEmpty())
				result.AddMessage("ClubId cannot be null");

			if (request.EventId.IsEmpty())
				result.AddMessage("EventId cannot be null");

			return result;
		}
	}
}

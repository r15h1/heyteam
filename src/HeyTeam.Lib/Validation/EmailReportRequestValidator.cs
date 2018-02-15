﻿using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Validation
{
	public class EmailReportRequestValidator : IValidator<EmailReportRequest> {
		public ValidationResult<EmailReportRequest> Validate(EmailReportRequest request) {
			var result = new ValidationResult<EmailReportRequest>(request);
			if (request == null) {
				result.AddMessage("Request cannot be null");
			} else {
				if (request.ClubId.IsEmpty())
					result.AddMessage("ClubId cannot be null");

				if (request.EventId.IsEmpty())
					result.AddMessage("EventId cannot be null");

				foreach (var email in request.EmailAddresses)
					if(!email.IsEmpty() && !email.IsValidEmail())
							result.AddMessage($"{email} is not a valid email");
			}
			return result;
		}
	}
}

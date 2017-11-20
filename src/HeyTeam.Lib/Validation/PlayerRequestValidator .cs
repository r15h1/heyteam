using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Validation {
	public class PlayerRequestValidator : IValidator<PlayerRequest>  {
        public ValidationResult<PlayerRequest> Validate(PlayerRequest request) {
            var result = new ValidationResult<PlayerRequest>(request);
			
			if (request.SquadId.IsEmpty()) 
                result.AddMessage("Squad Id cannot be empty");

            if (request.FirstName.IsEmpty()) 
                result.AddMessage("First Name cannot be empty");

            if (request.LastName.IsEmpty()) 
                result.AddMessage("Last Name cannot be empty");

            if (request.Email.IsEmpty() || !request.Email.IsValidEmail()) 
                result.AddMessage("Email is not valid");

            if (request.DominantFoot.IsEmpty()) {
                result.AddMessage("Dominant foot cannot be empty");
            } else if (!char.ToUpperInvariant(request.DominantFoot).Equals('R') && 
                        !char.ToUpperInvariant(request.DominantFoot).Equals('L')) {
                result.AddMessage("Dominant foot can be \"R\" (right) or \"L\" (left) only");
            }            

            if (request.Nationality.IsEmpty()) 
                result.AddMessage("Nationality cannot be empty");

            if (!request.DateOfBirth.HasValue) 
                result.AddMessage("Date of Birth cannot be empty");
            else if (request.DateOfBirth.Value > DateTime.Now) 
                result.AddMessage("Date of Birth cannot be in the future");
                        
            return result;
        }
    }
}
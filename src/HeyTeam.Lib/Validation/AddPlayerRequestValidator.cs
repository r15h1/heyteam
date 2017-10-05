using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
    public class AddPlayerRequestValidator : IValidator<AddPlayerRequest>
    {
        public ValidationResult<AddPlayerRequest> Validate(AddPlayerRequest request)
        {
            var result = new ValidationResult<AddPlayerRequest>(request);
            if (request.SquadId.IsEmpty()) 
                result.AddMessage("Squad Id cannot be empty");

            if (request.FirstName.IsEmpty()) 
                result.AddMessage("First Name cannot be empty");

            if (request.LastName.IsEmpty()) 
                result.AddMessage("First Name cannot be empty");
            
            return result;
        }
    }
}
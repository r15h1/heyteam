using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
    public class AddSquadRequestValidator : IValidator<AddSquadRequest>
    {
        public ValidationResult<AddSquadRequest> Validate(AddSquadRequest request)
        {
            var result = new ValidationResult<AddSquadRequest>(request);
            if (request.ClubId.IsEmpty()) 
                result.AddMessage("Club Id cannot be empty");

            if (request.SquadName.IsEmpty()) 
                result.AddMessage("Squad name cannot be empty");

            return result;
        }
    }
}
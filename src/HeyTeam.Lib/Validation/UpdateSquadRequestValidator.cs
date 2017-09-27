using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
    public class UpdateSquadRequestValidator : IValidator<UpdateSquadRequest>
    {
        public ValidationResult<UpdateSquadRequest> Validate(UpdateSquadRequest request)
        {
            var result = new ValidationResult<UpdateSquadRequest>(request);
            if (request.ClubId.IsEmpty()) 
                result.AddMessage("Club Id cannot be empty");

            if (request.SquadId.IsEmpty()) 
                result.AddMessage("Squad Id cannot be empty");

            if (string.IsNullOrWhiteSpace(request.SquadName)) 
                result.AddMessage("Squad name cannot be empty");

            return result;
        }
    }
}
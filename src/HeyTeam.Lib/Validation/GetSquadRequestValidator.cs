using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Validation {
    public class GetSquadRequestValidator : IValidator<GetSquadRequest>
    {
        public ValidationResult<GetSquadRequest> Validate(GetSquadRequest request)
        {
            var result = new ValidationResult<GetSquadRequest>(request);            

            return result;
        }
    }
}
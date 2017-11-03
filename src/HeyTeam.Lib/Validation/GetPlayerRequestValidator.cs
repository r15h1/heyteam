using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation
{
	public class GetPlayerRequestValidator : IValidator<GetPlayerRequest>
	{
		public ValidationResult<GetPlayerRequest> Validate(GetPlayerRequest request)
		{
			var result = new ValidationResult<GetPlayerRequest>(request);
			if (request.PlayerId.IsEmpty())
				result.AddMessage("Player Id cannot be empty");

			return result;
		}
	}
}

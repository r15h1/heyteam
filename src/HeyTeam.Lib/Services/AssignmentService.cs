using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Services {
	public class AssignmentService : IAssignmentService {
		private readonly IValidator<AssignmentRequest> assignementRequestValidator;

		public AssignmentService(IValidator<AssignmentRequest> assignementRequestValidator){
			this.assignementRequestValidator = assignementRequestValidator;
		}

		public Response CreateAssignment(AssignmentRequest request) {
			var result = assignementRequestValidator.Validate(request);
			if (!result.IsValid)
				return Response.CreateResponse(result.Messages);

			

			return Response.CreateResponse(new System.NotImplementedException());
		}
	}
}

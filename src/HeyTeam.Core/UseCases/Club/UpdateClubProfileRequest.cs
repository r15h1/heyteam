using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class UpdateClubProfileRequest {
        public Guid ClubId { get; set; }
        public string ClubName { get; set; }
        public string LogoUrl { get; set; }
    }

    public class UpdateClubProfileResponse {
        public UpdateClubProfileResponse (ValidationResult<UpdateClubProfileRequest> validationResult) {
            this.ValidationResult = validationResult;
        }

        public ValidationResult<UpdateClubProfileRequest> ValidationResult { get; private set; }
    }
}
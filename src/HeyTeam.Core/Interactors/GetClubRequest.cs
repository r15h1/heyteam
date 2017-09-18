using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class GetClubRequest {
        public long? ClubId { get; set; }
        public string NameStartsWith { get; set; }
    }

    public class GetClubResponse {
        public GetClubResponse(ValidationResult<GetClubRequest> validationResult, IList<Club> clubs) {
            Clubs = clubs;
            ValidationResult = validationResult;
        }
        public IList<Club> Clubs { get; private set; }        
        public ValidationResult<GetClubRequest> ValidationResult { get; private set; }
    }
}
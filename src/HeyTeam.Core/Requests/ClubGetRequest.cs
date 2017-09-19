using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Requests {
    public class ClubGetRequest {
        public long? ClubId { get; set; }
        public string NameStartsWith { get; set; }
    }

    public class ClubGetResponse {
        public ClubGetResponse(ValidationResult<ClubGetRequest> validationResult, IList<Club> clubs) {
            Clubs = clubs;
            ValidationResult = validationResult;
        }
        public IList<Club> Clubs { get; private set; }        
        public ValidationResult<ClubGetRequest> ValidationResult { get; private set; }
    }
}
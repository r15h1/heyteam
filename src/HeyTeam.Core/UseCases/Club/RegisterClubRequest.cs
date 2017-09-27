using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class RegisterClubRequest {        
        public string ClubName { get; set; }
        public string ClubLogoUrl { get; set; }
    }

    public class RegisterClubResponse : Response {                
        public RegisterClubResponse(Guid? guid = null):base() {
            this.ClubId = guid;
        }

        public Guid? ClubId { get; }                
    }
}
using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class RegisterClubRequest {        
        public string ClubName { get; set; }
        public string ClubLogoUrl { get; set; }
    }
}
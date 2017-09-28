using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class UpdateClubProfileRequest {
        public Guid ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubLogoUrl { get; set; }
    }
}
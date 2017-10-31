using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class RegisterClubRequest {        
        public string Name { get; set; }
        public string Url { get; set; }        
    }
}
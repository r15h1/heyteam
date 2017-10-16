using System;
using System.Collections.Generic;

namespace HeyTeam.Core.UseCases {
    public class DashboardRequest {        
        public string UserEmail { get; set; }
        public Guid ClubId { get; set; }
    }
}
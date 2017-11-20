using System;
using System.Collections.Generic;
using HeyTeam.Core.Dashboard;

namespace HeyTeam.Core.Queries {
    public interface IDashboardQuery {
        (List<Group> Dashboard, IEnumerable<string> Errors) GetDashboard(DashboardRequest request);
    }

	public class DashboardRequest {
		public Guid ClubId { get; set; }
		public string UserEmail { get; set; }
	}

}
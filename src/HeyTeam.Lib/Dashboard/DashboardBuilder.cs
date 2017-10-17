using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Repositories;
using HeyTeam.Identity;

namespace HeyTeam.Lib.Dashboard {
    public class DashboardBuilder : AbstractDashboardBuilder
    {
        public DashboardBuilder(IDashboardRepository dashboardRepository, IIdentityManager identityManager) : base(dashboardRepository, identityManager)
        {
        }

        public override List<Group> Build()
        {       
            List<Group> dashboard = new List<Group>();     
            var roles = identityManager.GetRoles(base.UserEmail);
            if(roles == null || roles.Count() == 0) return dashboard;

            if(roles.Contains(Roles.Administrator.ToString()) || roles.Contains(Roles.Coach.ToString()))
                dashboard.Add(GetSquadSummary(UserEmail));

            if(roles.Contains(Roles.Player.ToString()) || roles.Contains(Roles.Coach.ToString()))
                dashboard.Add(GetActionItems(UserEmail));

            dashboard.Add(GetUpcomingEvents(UserEmail));
            dashboard.Add(GetActivityLog(UserEmail));
            
            return dashboard;
        }

        private Group GetSquadSummary(string userEmail)
        {
            throw new NotImplementedException();
        }

        private Group GetActionItems(string userEmail)
        {
            throw new NotImplementedException();
        }
        
        private Group GetUpcomingEvents(string userEmail)
        {
            throw new NotImplementedException();
        }

        private Group GetActivityLog(string userEmail)
        {
            throw new NotImplementedException();
        }        
    }
}
using System;
using System.Collections.Generic;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Repositories;

namespace HeyTeam.Core.Dashboard {
    public abstract class AbstractDashboardBuilder
    {
        protected readonly IDashboardRepository dashboardRepository;
        protected readonly IIdentityManager identityManager;

        public AbstractDashboardBuilder(IDashboardRepository dashboardRepository, IIdentityManager identityManager) {
            this.dashboardRepository = dashboardRepository;
            this.identityManager = identityManager;
        }

        public string UserEmail { get; internal set; }
        public Guid ClubId { get; internal set; }

        public abstract List<Group> Build();
    }
}
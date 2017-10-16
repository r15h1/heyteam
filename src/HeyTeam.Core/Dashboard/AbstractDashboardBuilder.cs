using System;
using System.Collections.Generic;
using HeyTeam.Core.Repositories;

namespace HeyTeam.Core.Dashboard {
    public abstract class AbstractDashboardBuilder
    {
        protected readonly IDashboardRepository dashboardRepository;
        protected readonly IUserRepository userRepository;

        public AbstractDashboardBuilder(IDashboardRepository dashboardRepository, IUserRepository userRepository) {
            this.dashboardRepository = dashboardRepository;
            this.userRepository = userRepository;
        }

        public string UserEmail { get; internal set; }
        public Guid ClubId { get; internal set; }

        public abstract List<Group> Build();
    }
}
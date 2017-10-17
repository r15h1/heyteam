using System.Collections.Generic;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;

namespace HeyTeam.Lib.Repositories {
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public DashboardRepository(IDbConnectionFactory connectionFactory) {
            this.connectionFactory = connectionFactory;
        }
        public List<Item> GetSquads(string user)
        {
            throw new System.NotImplementedException();
        }
    }
}
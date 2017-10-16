using System.Collections.Generic;
using HeyTeam.Core.Dashboard;

namespace HeyTeam.Core.Repositories {
    public interface IDashboardRepository {
        List<Item> GetSquads(string user);
    }
}
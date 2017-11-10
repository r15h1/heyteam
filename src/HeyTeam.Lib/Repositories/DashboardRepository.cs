using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public DashboardRepository(IDbConnectionFactory connectionFactory) {
            Ensure.ArgumentNotNull(connectionFactory);
            this.connectionFactory = connectionFactory;
        }
        public List<Item> GetSquadSummary(Guid clubdId)
        {
            using(var connection = connectionFactory.Connect()) {
                string sql = @"SELECT S.Name,
                                S.Guid,
                                (SELECT COUNT(1) FROM Players P WHERE P.SquadId = S.SquadId) AS NumberOfPlayers,
								(SELECT CO.FirstName + ' ' + CO.LastName FROM SquadCoaches SC INNER JOIN Coaches CO ON SC.SquadId = S.SquadId AND CO.CoachId = SC.CoachId) AS Coach
                            FROM Clubs C
                            INNER JOIN Squads S ON S.ClubId = C.ClubId
                            WHERE C.Guid = @ClubGuid";                                
                var p = new DynamicParameters();
                p.Add("@ClubGuid", clubdId.ToString());
                connection.Open();
                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                return reader.Select<dynamic, Item>(row => BuildItem(row)).ToList();
            }
        }

        private Item BuildItem(dynamic row)
        {
            var item = new Item();            
            item.Cells.Add("squadname", row.Name);
            item.Cells.Add("squadid", row.Guid.ToString());
            item.Cells.Add("numberofplayers", row.NumberOfPlayers.ToString());
			item.Cells.Add("coach", row.Coach);
			return item;
        }
    }
}
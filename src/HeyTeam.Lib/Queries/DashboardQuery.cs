using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using HeyTeam.Core.Validation;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Queries;

namespace HeyTeam.Lib.Queries {
    public class DashboardQuery : IDashboardQuery
    {
        private readonly IDbConnectionFactory connectionFactory;
		private readonly IIdentityManager identityManager;
		private readonly IValidator<DashboardRequest> validator;

		public DashboardQuery(IDbConnectionFactory connectionFactory, IIdentityManager identityManager, IValidator<DashboardRequest> validator) {
            ThrowIf.ArgumentIsNull(connectionFactory);
            this.connectionFactory = connectionFactory;
			this.identityManager = identityManager;
			this.validator = validator;
		}		

		public (List<Group> Dashboard, IEnumerable<string> Errors) GetDashboard(DashboardRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return (null, new List<string>(validationResult.Messages));

			return (BuildDashboard(request.ClubId, request.UserEmail), null);
		}

		private List<Group> BuildDashboard(Guid clubId, string email) {
			var roles = identityManager.GetRoles(email);
			var dashboard = new List<Group>();

			if (roles.Contains(Roles.Administrator))
				dashboard.Add(GetSquadSummary(clubId));

			return dashboard;
		}

		private Group GetSquadSummary(Guid clubId) {
			return new Group {
				Name = "squads",
				Items = Query(clubId)
			};
		}

		private List<Item> Query(Guid clubdId) {
			using (var connection = connectionFactory.Connect()) {
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

		private Item BuildItem(dynamic row) {
			var item = new Item();
			item.Cells.Add("squadname", row.Name);
			item.Cells.Add("squadid", row.Guid.ToString());
			item.Cells.Add("numberofplayers", row.NumberOfPlayers.ToString());
			item.Cells.Add("coach", row.Coach);
			return item;
		}
	}
}
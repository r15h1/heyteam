using System.Collections.Generic;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Core.Entities;
using Dapper;
using HeyTeam.Util;
using HeyTeam.Core.Queries;

namespace HeyTeam.Lib.Repositories {
    public class SquadRepository : ISquadRepository {
        private readonly IDbConnectionFactory connectionFactory;
		private readonly IClubQuery clubRepository;

		public SquadRepository(IDbConnectionFactory factory, IClubQuery clubRepository) {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
			this.clubRepository = clubRepository;
		}

        public void AddSquad(Squad squad) {
            using(var connection = connectionFactory.Connect()) {
            string sql =    @"INSERT INTO SQUADS(ClubId, Guid, Name) 
                                SELECT C.ClubId, @SquadGuid, @Name FROM CLUBS C  
                                    WHERE C.Guid = @ClubGuid";  
                                
                var p = new DynamicParameters();
                p.Add("@SquadGuid", squad.Guid.ToString());
                p.Add("@Name", squad.Name);
                p.Add("@ClubGuid", squad.ClubId.ToString());
                connection.Open();
                connection.Execute(sql, p);
            }
        }

		public void AssignCoach(Guid squadId, Guid coachId) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @" DELETE FROM SquadCoaches WHERE SquadId = (SELECT SquadId FROM Squads WHERE Guid = @SquadGuid);
				
								INSERT INTO SquadCoaches(SquadId, CoachId) 
								VALUES (
									(SELECT SquadId FROM Squads WHERE Guid = @SquadGuid),
									(SELECT CoachId FROM Coaches WHERE Guid = @CoachGuid)
								);";

				var p = new DynamicParameters();
				p.Add("@SquadGuid", squadId.ToString());
				p.Add("@CoachGuid", coachId.ToString());
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void UnAssignCoach(Guid squadId, Guid coachId) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @" DELETE FROM SquadCoaches WHERE 
								SquadId = (SELECT SquadId FROM Squads WHERE Guid = @SquadGuid) 
								AND CoachId = (SELECT CoachId FROM Coaches WHERE Guid = @CoachGuid)";

				var p = new DynamicParameters();
				p.Add("@SquadGuid", squadId.ToString());
				p.Add("@CoachGuid", coachId.ToString());
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void UpdateSquad(Squad squad) {
            using(var connection = connectionFactory.Connect()) {
                string sql = @"UPDATE Squads SET Name = @Name WHERE Guid = @Guid";  
                                
                var p = new DynamicParameters();
                p.Add("@Guid", squad.Guid.ToString());
                p.Add("@Name", squad.Name);
                connection.Open();
                connection.Execute(sql, p);
            }
        }
    }
}
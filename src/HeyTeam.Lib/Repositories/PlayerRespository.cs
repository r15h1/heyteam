using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
	public class PlayerRepository : IPlayerRepository {

        private readonly IDbConnectionFactory connectionFactory;
		private readonly IMemberQuery memberQuery;

		public PlayerRepository(IDbConnectionFactory factory, IMemberQuery memberQuery) {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
			this.memberQuery = memberQuery;
		}

        public void AddPlayer(Player player) {
            ThrowIf.ArgumentIsNull(player);
            using(var connection = connectionFactory.Connect())
            {
                string sql = GetInsertStatement();
                DynamicParameters p = SetupInsertParameters(player);
                connection.Open();
                connection.Execute(sql, p);                
            }
			memberQuery.UpdateCache();
        }

		public void UpdatePlayer(Player player)
		{
			ThrowIf.ArgumentIsNull(player);
			using (var connection = connectionFactory.Connect())
			{
				string sql = GetUpdateStatement();
				DynamicParameters p = SetupUpdateParameters(player);
				connection.Open();
				connection.Execute(sql, p);
			}
			memberQuery.UpdateCache();
		}

		private string GetInsertStatement() {
            return @"INSERT INTO PLAYERS (
                SquadId, Guid, DateOfBirth, 
                DominantFoot, FirstName, LastName, Email, Nationality, SquadNumber
            ) 
            SELECT S.SquadId, @PlayerGuid, @DateOfBirth, 
                @DominantFoot, @FirstName, @LastName, @Email, @Nationality, @SquadNumber
            FROM SQUADS S
            WHERE S.Guid = @SquadGuid";


        }

        private DynamicParameters SetupInsertParameters(Player player) {
            var p = new DynamicParameters();
            p.Add("@SquadGuid", player.SquadId.ToString());
            p.Add("@PlayerGuid", player.Guid.ToString());
            p.Add("@DateOfBirth", player.DateOfBirth);
            p.Add("@DominantFoot", player.DominantFoot.ToString());
            p.Add("@FirstName", player.FirstName);
            p.Add("@LastName", player.LastName);
            p.Add("@Email", player.Email);
            p.Add("@Nationality", player.Nationality);
			p.Add("@SquadNumber", player.SquadNumber);
			return p;
        }

		private string GetUpdateStatement()
		{
			return @"UPDATE PLAYERS SET DateOfBirth =@DateOfBirth, 
						DominantFoot = @DominantFoot,
						FirstName = @FirstName,       
						LastName = @LastName, 
						Email = @Email, 
						Nationality = @Nationality, 
						SquadNumber = @SquadNumber                 
            WHERE Guid = @PlayerGuid  AND (Deleted IS NULL OR Deleted = 0)";
		}

		private DynamicParameters SetupUpdateParameters(Player player)
		{
			var p = new DynamicParameters();
			p.Add("@PlayerGuid", player.Guid.ToString());
			p.Add("@DateOfBirth", player.DateOfBirth);
			p.Add("@DominantFoot", player.DominantFoot.ToString());
			p.Add("@FirstName", player.FirstName);
			p.Add("@LastName", player.LastName);
			p.Add("@Email", player.Email);
			p.Add("@Nationality", player.Nationality);
			p.Add("@SquadNumber", player.SquadNumber);
			return p;
		}

		public void DeletePlayer(Player player) {
			ThrowIf.ArgumentIsNull(player);
			using (var connection = connectionFactory.Connect()) {
				string sql = "UPDATE PLAYERS SET Deleted=1, DeletedOn=GetDate() WHERE Guid = @PlayerGuid";
				var p = new DynamicParameters();
				p.Add("@PlayerGuid", player.Guid.ToString());
				connection.Open();
				connection.Execute(sql, p);
			}
			memberQuery.UpdateCache();
		}
	}
}
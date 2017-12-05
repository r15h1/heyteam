using HeyTeam.Lib.Data;
using HeyTeam.Lib.Settings;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace HeyTeam.Lib.Repositories {
	public class ConnectionFactory : IDbConnectionFactory
    {

        public ConnectionFactory(IOptions<DatabaseSettings> settings) => this.DatabaseSettings = settings.Value ;

        private DatabaseSettings DatabaseSettings { get; }

        public IDbConnection Connect() {
            return new SqlConnection(DatabaseSettings.ConnectionString);
        }
    }
}
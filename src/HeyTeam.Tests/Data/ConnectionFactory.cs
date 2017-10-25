using System;
using System.Data;
using HeyTeam.Lib.Data;
using Microsoft.Data.Sqlite;

namespace HeyTeam.Tests.Data
{
    public class ConnectionFactory : IDbConnectionFactory
    {
        public ConnectionFactory(DatabaseSettings settings) => Settings = settings;

        public DatabaseSettings Settings{ get; }

        public IDbConnection Connect()
        {
            return new SqliteConnection(Settings.ConnectionString);
        }
    }
}
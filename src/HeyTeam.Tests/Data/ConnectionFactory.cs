using System;
using System.Data;
using HeyTeam.Lib.Data;
using Microsoft.Data.Sqlite;

namespace HeyTeam.Tests.Data
{
    public class ConnectionFactory : IDbConnectionFactory
    {
        public ConnectionFactory(string connectionString) => ConnectionString = connectionString;

        public string ConnectionString{ get; }

        public IDbConnection Connect()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
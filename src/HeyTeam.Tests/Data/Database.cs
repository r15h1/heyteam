using System.IO;
using System.Data;
using Dapper;
using HeyTeam.Lib.Data;

namespace HeyTeam.Tests.Data
{
    public static class Database {
        public static void Create(string connectionString) {
            string scriptPath = @"SqliteScript.sql";
            FileInfo file = new FileInfo(scriptPath);
            string script = file.OpenText().ReadToEnd();

            using (var connection = new ConnectionFactory(new DatabaseSettings{ ConnectionString = connectionString } ).Connect()) {
                connection.Open();
                connection.Execute(script);
            }
        }
    }
}

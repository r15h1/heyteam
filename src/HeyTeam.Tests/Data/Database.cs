using System.IO;
using System.Data;
using Dapper;
namespace HeyTeam.Tests.Data
{
    public static class Database {
        public static void Create(string connectionString) {
            string scriptPath = @"./SqliteScript.sql";
            FileInfo file = new FileInfo(scriptPath);
            string script = file.OpenText().ReadToEnd();

            using (var connection = new ConnectionFactory(connectionString).Connect()) {
                connection.Open();
                connection.Execute(script);
            }
        }
    }
}
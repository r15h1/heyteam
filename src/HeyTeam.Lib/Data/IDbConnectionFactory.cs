using System.Data;

namespace HeyTeam.Lib.Data {
    public interface IDbConnectionFactory {
        IDbConnection Connect();
    }
}
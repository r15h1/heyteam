using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;

namespace HeyTeam.Lib.Repositories {
    public class PlayerRepository : IPlayerRepository {

        private readonly IDbConnectionFactory connectionFactory;
        public PlayerRepository(IDbConnectionFactory factory) {
            this.connectionFactory = factory;
        }
    }
}
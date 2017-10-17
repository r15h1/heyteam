using System.Collections.Generic;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;

namespace HeyTeam.Lib.Repositories {
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory) {
            this.connectionFactory = connectionFactory;
        }

        public IEnumerable<string> GetRoles(string userEmail)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System.Collections.Generic;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Data;

namespace HeyTeam.Lib.Repositories {
    public class SquadRepository : ISquadRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public SquadRepository(IDbConnectionFactory factory) {
            this.connectionFactory = factory;
        }

        public void Add(Core.Entities.Squad squad)
        {
            throw new NotImplementedException();
        }
    }
}
using System;

namespace HeyTeam.Core.Entities {
    public class Squad {
        public Squad(Club club) {
            if(club == null || !club.Id.HasValue) throw new ArgumentNullException();

            Club = club;
        }
        public Club Club { get; private set; }
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}
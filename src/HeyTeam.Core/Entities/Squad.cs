using System;

namespace HeyTeam.Core.Entities {
    public class Squad {
        public Squad(Club club, Guid? id = null) {
            if(club == null) throw new ArgumentNullException();

            Club = club;
            Id = id.HasValue ? id.Value : System.Guid.NewGuid();
        }
        public Club Club { get; private set; }
        public Guid Id { get; private set; }
        public string Name { get; set; }
    }
}